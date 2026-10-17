function summary = open_loop_compare(dataPaths, varargin)
% OPEN_LOOP_COMPARE 开环输出逼近：把 PPO 采集 CSV 的四维磁特征送入 FuzzyController，
% 与 CSV 中的 output_vx_norm、output_omega_norm 对比。
%
% 用法:
%   open_loop_compare({'a.csv', 'b.csv', 'c.csv'})
%   open_loop_compare('<放有这些csv的文件夹>')
%   open_loop_compare(dataPaths, 'fuzzy', 'FuzzyController.cs 路径', 'out', '输出目录')
%   命令行: matlab -batch "open_loop_compare('D:/data')"
%
% CSV 需含列:
%   front_lr_diff, rear_lr_diff, front_center, rear_center,
%   output_vx_norm, output_omega_norm
%
% 主指标使用规则加权平均（不含低置信度恢复），对应“规则库逼近教师输出”。
% 同时给出含恢复混合的部署输出，便于和车上实际 FuzzyController.Evaluate 对照。

    INPUT_COLS = {'front_lr_diff', 'rear_lr_diff', 'front_center', 'rear_center'};
    TEACHER_COLS = {'output_vx_norm', 'output_omega_norm'};

    here = fileparts(mfilename('fullpath'));
    p = inputParser;
    p.addRequired('dataPaths');
    p.addParameter('fuzzy', fullfile(here, 'FuzzyController.cs'));
    p.addParameter('out', '');
    p.parse(dataPaths, varargin{:});
    fuzzyPath = char(p.Results.fuzzy);
    outDir = char(p.Results.out);
    dataPaths = cellstr(dataPaths);

    if ~isfile(fuzzyPath)
        error('找不到规则库文件: %s', fuzzyPath);
    end
    [mf, rules] = load_fuzzy(fuzzyPath);
    fprintf('已加载规则库: %d 条规则, %s\n', size(rules, 1), fuzzyPath);

    csvs = collect_csvs(dataPaths);
    if isempty(csvs)
        error('没有找到 csv: %s', strjoin(dataPaths, ', '));
    end

    if isempty(outDir)
        if numel(dataPaths) == 1 && isfolder(dataPaths{1})
            outDir = fullfile(dataPaths{1}, 'open_loop_out');
        else
            outDir = fullfile(fileparts(get_abs_path(csvs{1})), 'open_loop_out');
        end
    end
    if ~isfolder(outDir)
        mkdir(outDir);
    end

    setup_font();
    rows = struct([]);
    allTvx = {}; allFvx = {}; allTw = {}; allFw = {};

    for k = 1:numel(csvs)
        path = csvs{k};
        [~, name, ext] = fileparts(path);
        data = load_teacher_csv(path, [INPUT_COLS, TEACHER_COLS]);
        if isempty(data)
            fprintf('跳过（列数不足 13 列）: %s%s\n', name, ext);
            continue;
        end
        front = data(:, 1);
        rear = data(:, 2);
        fc = data(:, 3);
        rc = data(:, 4);
        tvx = data(:, 5);
        tw = data(:, 6);
        [fvx, fw] = evaluate_batch(front, rear, fc, rc, mf, rules, false);
        [dvx, dw] = evaluate_batch(front, rear, fc, rc, mf, rules, true);

        [maeVx, rmseVx, p95Vx, maxVx] = metrics(tvx, fvx);
        [maeW, rmseW, p95W, maxW] = metrics(tw, fw);
        [maeVxD, rmseVxD] = metrics(tvx, dvx);
        [maeWD, rmseWD] = metrics(tw, dw);

        row = make_row(name, numel(tvx), maeVx, rmseVx, p95Vx, maxVx, ...
                       maeW, rmseW, p95W, maxW, maeVxD, rmseVxD, maeWD, rmseWD);
        rows = [rows, row]; %#ok<AGROW>
        allTvx{end + 1} = tvx; %#ok<AGROW>
        allFvx{end + 1} = fvx; %#ok<AGROW>
        allTw{end + 1} = tw; %#ok<AGROW>
        allFw{end + 1} = fw; %#ok<AGROW>
        plot_series(tvx, fvx, tw, fw, fullfile(outDir, [name '_series.png']), ...
                    [name ' 开环输出'], 4000);
        fprintf('%s: N=%d  RMSE(vx)=%.4f  MAE(vx)=%.4f  RMSE(w)=%.4f  MAE(w)=%.4f\n', ...
                name, numel(tvx), rmseVx, maeVx, rmseW, maeW);
    end

    if isempty(rows)
        error('没有可用的蒸馏 CSV。需要含 output_vx_norm 与四维磁特征列。');
    end

    tvx = vertcat(allTvx{:});
    fvx = vertcat(allFvx{:});
    tw = vertcat(allTw{:});
    fw = vertcat(allFw{:});
    [maeVx, rmseVx, p95Vx, maxVx] = metrics(tvx, fvx);
    [maeW, rmseW, p95W, maxW] = metrics(tw, fw);
    perFile = rows;
    rows = [rows, make_row('ALL', numel(tvx), maeVx, rmseVx, p95Vx, maxVx, ...
                           maeW, rmseW, p95W, maxW, NaN, NaN, NaN, NaN)];
    summary = struct2table(rows);

    csvPath = fullfile(outDir, 'open_loop_summary.csv');
    write_summary(rows, csvPath);

    plot_scatter(tvx, fvx, tw, fw, fullfile(outDir, 'open_loop_scatter.png'), ...
                 '开环输出散点（规则加权平均）');
    plot_summary_bars(perFile, fullfile(outDir, 'open_loop_bars.png'));

    fprintf('\n开环逼近（规则加权平均，不含低置信度恢复）\n');
    fprintf('%-28s %8s %8s %8s %8s %8s\n', 'File', 'N', 'RMSEvx', 'MAEvx', 'RMSEw', 'MAEw');
    for k = 1:numel(rows)
        r = rows(k);
        fprintf('%-28s %8d %8.4f %8.4f %8.4f %8.4f\n', r.file, r.n, ...
                r.rmse_vx, r.mae_vx, r.rmse_omega, r.mae_omega);
    end
    fprintf('\n汇总表: %s\n', csvPath);
    fprintf('散点图: %s\n', fullfile(outDir, 'open_loop_scatter.png'));
    fprintf('柱状图: %s\n', fullfile(outDir, 'open_loop_bars.png'));
    fprintf('各文件时序图: <文件名>_series.png\n');
    fprintf('论文主指标用 RMSEvx / MAEvx / RMSEw / MAEw。with_recovery 列是部署时含恢复混合的误差。\n');
end


function nums = extract_block(text, name)
    tok = regexp(text, [name '\s*=\s*\{(.*?)\};'], 'tokens', 'once');
    if isempty(tok)
        error('在 FuzzyController.cs 中找不到 %s', name);
    end
    block = regexprep(tok{1}, '//[^\n]*', '');
    block = strrep(block, 'f', '');
    strs = regexp(block, '[-+]?(?:\d+\.\d+|\d+)', 'match');
    nums = str2double(strs(:));
end


function [mf, rules] = load_fuzzy(csPath)
    text = fileread(csPath);
    if ~isempty(text) && text(1) == char(65279)
        text = text(2:end);
    end
    mf.front = reshape(extract_block(text, 'MF_FrontLR'), 3, [])';
    mf.rear = reshape(extract_block(text, 'MF_RearLR'), 3, [])';
    mf.fc = reshape(extract_block(text, 'MF_FrontCenter'), 3, [])';
    mf.rc = reshape(extract_block(text, 'MF_RearCenter'), 3, [])';
    rules = reshape(extract_block(text, 'Rules'), 7, [])';
end


function out = tri_mf(x, abc)
% 与 FuzzyController.TriMF 一致。x 为列向量。
    a = abc(1); b = abc(2); c = abc(3);
    if a >= b - 1e-6
        out = (c - x) / max(1e-6, c - b);
        out(x >= c) = 0.0;
        out(x <= b) = 1.0;
        return;
    end
    if b >= c - 1e-6
        out = (x - a) / max(1e-6, b - a);
        out(x <= a) = 0.0;
        out(x >= b) = 1.0;
        return;
    end
    left = (x - a) / max(1e-6, b - a);
    right = (c - x) / max(1e-6, c - b);
    out = right;
    out(x <= b) = left(x <= b);
    out(x <= a | x >= c) = 0.0;
end


function [vx, omega] = evaluate_batch(front, rear, fc, rc, mf, rules, withRecovery)
% 批量推理。返回 (vx, omega)，均为归一化 [-1, 1]。
    n = numel(front);
    sumVx = zeros(n, 1);
    sumW = zeros(n, 1);
    sumOmega = zeros(n, 1);
    bestW = -ones(n, 1);
    bestVx = zeros(n, 1);
    bestOmega = zeros(n, 1);

    for r = 1:size(rules, 1)
        rule = rules(r, :);
        mu1 = tri_mf(front, mf.front(rule(1) + 1, :));
        mu2 = tri_mf(rear, mf.rear(rule(2) + 1, :));
        mu3 = tri_mf(fc, mf.fc(rule(3) + 1, :));
        mu4 = tri_mf(rc, mf.rc(rule(4) + 1, :));
        firing = mu1 .* mu2 .* mu3 .* mu4;
        active = firing > 1e-6;
        w = firing * rule(7);
        sumVx(active) = sumVx(active) + w(active) * rule(5);
        sumOmega(active) = sumOmega(active) + w(active) * rule(6);
        sumW(active) = sumW(active) + w(active);
        better = active & (w > bestW);
        bestW(better) = w(better);
        bestVx(better) = rule(5);
        bestOmega(better) = rule(6);
    end

    hasWeight = sumW > 1e-6;
    vx = zeros(n, 1);
    omega = zeros(n, 1);
    vx(hasWeight) = sumVx(hasWeight) ./ max(sumW(hasWeight), 1e-12);
    omega(hasWeight) = sumOmega(hasWeight) ./ max(sumW(hasWeight), 1e-12);
    useBest = ~hasWeight & (bestW > 0.0);
    vx(useBest) = bestVx(useBest);
    omega(useBest) = bestOmega(useBest);
    vx = clip(vx, -1.0, 1.0);
    omega = clip(omega, -1.0, 1.0);

    if withRecovery
        trackConf = clip(0.5 * (fc + rc), 0.0, 1.0);
        lateralErr = 0.55 * front + 0.45 * rear;
        headingErr = front - rear;
        recoverVx = clip(-0.85 * lateralErr, -1.0, 1.0);
        recoverOmega = clip(-(1.15 * lateralErr + 0.75 * headingErr), -1.0, 1.0);
        confBlend = clip((0.65 - trackConf) / 0.35, 0.0, 1.0);
        weightBlend = clip((0.05 - sumW) / 0.05, 0.0, 1.0);
        blend = max(confBlend, weightBlend);
        vx = (1.0 - blend) .* vx + blend .* recoverVx;
        omega = (1.0 - blend) .* omega + blend .* recoverOmega;
        vx = clip(vx, -1.0, 1.0);
        omega = clip(omega, -1.0, 1.0);
    end
end


function y = clip(x, lo, hi)
    y = min(max(x, lo), hi);
end


function [mae, rmse, p95, maxAbs] = metrics(teacher, pred)
    err = abs(pred - teacher);
    mae = mean(err);
    rmse = sqrt(mean(err .^ 2));
    maxAbs = max(err);
    p95 = quantile_linear(err, 0.95);
end


function q = quantile_linear(v, p)
% 与 numpy.quantile 默认的线性插值一致。
    v = sort(v(:));
    pos = (numel(v) - 1) * p + 1;
    lo = floor(pos);
    hi = min(lo + 1, numel(v));
    q = v(lo) + (pos - lo) * (v(hi) - v(lo));
end


function files = collect_csvs(paths)
    files = {};
    for k = 1:numel(paths)
        path = paths{k};
        if isfile(path)
            files{end + 1} = path; %#ok<AGROW>
        elseif isfolder(path)
            listing = dir(fullfile(path, '*.csv'));
            listing = listing(~[listing.isdir]);
            names = sort({listing.name});
            for j = 1:numel(names)
                files{end + 1} = fullfile(path, names{j}); %#ok<AGROW>
            end
        else
            fprintf('找不到路径，已跳过: %s\n', path);
        end
    end
end


function data = load_teacher_csv(path, cols)
    try
        T = readtable(path, 'VariableNamingRule', 'preserve');
    catch
        T = readtable(path);
    end
    % 不检查列名：有对应列名时按列名读取，否则按蒸馏记录的固定列位置读取
    % （第 1-4 列为四维磁特征，第 12、13 列为 output_vx_norm、output_omega_norm）
    FIXED_POS = [1, 2, 3, 4, 12, 13];
    names = T.Properties.VariableNames;
    data = zeros(height(T), numel(cols));
    for k = 1:numel(cols)
        idx = find(strcmp(names, cols{k}), 1);
        if isempty(idx)
            idx = FIXED_POS(k);
        end
        if idx > width(T)
            data = [];
            return;
        end
        v = T{:, idx};
        if ~isnumeric(v)
            v = str2double(string(v));
        end
        data(:, k) = double(v);
    end
    data = data(~any(isnan(data), 2), :);
end


function full = get_abs_path(path)
    if java.io.File(path).isAbsolute()
        full = path;
    else
        full = fullfile(pwd, path);
    end
end


function row = make_row(name, n, maeVx, rmseVx, p95Vx, maxVx, maeW, rmseW, p95W, maxW, ...
                        maeVxD, rmseVxD, maeWD, rmseWD)
    row = struct( ...
        'file', name, ...
        'n', n, ...
        'mae_vx', maeVx, ...
        'rmse_vx', rmseVx, ...
        'p95_vx', p95Vx, ...
        'max_vx', maxVx, ...
        'mae_omega', maeW, ...
        'rmse_omega', rmseW, ...
        'p95_omega', p95W, ...
        'max_omega', maxW, ...
        'mae_vx_with_recovery', maeVxD, ...
        'rmse_vx_with_recovery', rmseVxD, ...
        'mae_omega_with_recovery', maeWD, ...
        'rmse_omega_with_recovery', rmseWD);
end


function write_summary(rows, csvPath)
    fields = fieldnames(rows);
    fid = fopen(csvPath, 'w', 'n', 'UTF-8');
    cleaner = onCleanup(@() fclose(fid));
    fprintf(fid, '%s\n', strjoin(fields', ','));
    for k = 1:numel(rows)
        cells = cell(1, numel(fields));
        for j = 1:numel(fields)
            v = rows(k).(fields{j});
            if ischar(v)
                cells{j} = v;
            elseif strcmp(fields{j}, 'n')
                cells{j} = sprintf('%d', v);
            elseif isnan(v)
                cells{j} = '';
            else
                cells{j} = sprintf('%.6f', v);
            end
        end
        fprintf(fid, '%s\n', strjoin(cells, ','));
    end
end


function setup_font()
    fonts = listfonts;
    for name = {'Microsoft YaHei', 'SimHei'}
        if any(strcmpi(fonts, name{1}))
            set(groot, 'defaultAxesFontName', name{1});
            set(groot, 'defaultTextFontName', name{1});
            set(groot, 'defaultLegendFontName', name{1});
            break;
        end
    end
    set(groot, 'defaultTextInterpreter', 'none');
    set(groot, 'defaultLegendInterpreter', 'none');
    set(groot, 'defaultAxesTickLabelInterpreter', 'none');
end


function fig = new_figure(widthIn, heightIn)
    fig = figure('Visible', 'off', 'Color', 'w', 'Units', 'inches', ...
                 'Position', [1, 1, widthIn, heightIn], 'PaperPositionMode', 'auto');
end


function save_figure(fig, outPath)
    print(fig, outPath, '-dpng', '-r160');
    close(fig);
end


function plot_scatter(teacherVx, fuzzyVx, teacherW, fuzzyW, outPath, titleText)
    n = numel(teacherVx);
    step = max(1, floor(n / 8000));
    idx = 1:step:n;
    fig = new_figure(10, 4.6);
    pairs = {
        teacherVx(idx), fuzzyVx(idx), '横向速度 output_vx_norm';
        teacherW(idx), fuzzyW(idx), '角速度 output_omega_norm'};
    for k = 1:2
        ax = subplot(1, 2, k);
        hold(ax, 'on');
        scatter(ax, pairs{k, 1}, pairs{k, 2}, 6, 'filled', ...
                'MarkerFaceColor', [0.122, 0.467, 0.706], ...
                'MarkerFaceAlpha', 0.35, 'MarkerEdgeColor', 'none');
        plot(ax, [-1, 1], [-1, 1], 'k--', 'LineWidth', 1);
        xlim(ax, [-1.05, 1.05]);
        ylim(ax, [-1.05, 1.05]);
        axis(ax, 'square');
        xlabel(ax, 'PPO');
        ylabel(ax, '模糊规则库');
        title(ax, pairs{k, 3});
        grid(ax, 'on');
        ax.GridAlpha = 0.3;
    end
    sgtitle(fig, titleText);
    save_figure(fig, outPath);
end


function plot_series(teacherVx, fuzzyVx, teacherW, fuzzyW, outPath, titleText, maxPoints)
    n = numel(teacherVx);
    step = max(1, floor(n / maxPoints));
    idx = 1:step:n;
    x = idx - 1;
    fig = new_figure(10, 6.2);
    data = {teacherVx, fuzzyVx, 'vx_norm'; teacherW, fuzzyW, 'omega_norm'};
    axs = gobjects(2, 1);
    for k = 1:2
        ax = subplot(2, 1, k);
        axs(k) = ax;
        hold(ax, 'on');
        plot(ax, x, data{k, 1}(idx), 'LineWidth', 1.0, 'DisplayName', 'PPO');
        h = plot(ax, x, data{k, 2}(idx), 'LineWidth', 1.0, 'DisplayName', '模糊规则库');
        h.Color(4) = 0.85;
        ylabel(ax, data{k, 3});
        legend(ax, 'Location', 'northeast');
        grid(ax, 'on');
        ax.GridAlpha = 0.3;
    end
    xlabel(axs(2), '样本序号');
    linkaxes(axs, 'x');
    sgtitle(fig, titleText);
    save_figure(fig, outPath);
end


function plot_summary_bars(rows, outPath)
    names = {rows.file};
    m = numel(names);
    Y = [[rows.rmse_vx]', [rows.rmse_omega]', [rows.mae_vx]', [rows.mae_omega]'];
    fig = new_figure(max(8, 1.3 * m + 3), 4.8);
    ax = axes(fig);
    % 末尾补一行 NaN，避免只有一个文件时 bar 把 1x4 当成单序列
    bar(ax, 1:m + 1, [Y; NaN(1, 4)], 'grouped');
    xlim(ax, [0.4, m + 0.6]);
    set(ax, 'XTick', 1:m, 'XTickLabel', names);
    xtickangle(ax, 25);
    ylabel(ax, '归一化输出误差');
    title(ax, '开环输出逼近误差（规则加权平均）');
    legend(ax, {'RMSE vx', 'RMSE omega', 'MAE vx', 'MAE omega'}, 'Location', 'northeast');
    grid(ax, 'on');
    ax.XGrid = 'off';
    ax.GridAlpha = 0.3;
    save_figure(fig, outPath);
end
