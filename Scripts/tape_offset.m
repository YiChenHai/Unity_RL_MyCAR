%% 磁条横向偏差离线计算（带状模型拟合）
% 候选像素用三种线索：
%   1) 黄色：min(R,G) - B 相对亮度占比大，且 G 与 R 接近（不设亮度下限，阴影里的暗黄也算）；
%   2) 饱和且平整：R、G 都饱和，邻域内几乎全饱和（过曝时磁条成片纯白，地面夹着暗点）；
%   3) 淡黄：蓝色不比红、绿多，且明显比全图亮（光线暗、磁条发白时）。
% 再在传感器附近拟合一条直带：搜索角度和横向位置，取“带内多为候选像素、两侧边缘外很少”的那条。
% 偏差是传感器到带中心线的垂直距离。
% 磁条像素宽度先从抽样帧自动估计，毫米比例由磁条实际宽度换算，换相机、换分辨率不用改像素参数。
clear; clc; close all;

%% ===== 参数 =====
folder     = 'C:\Users\IFR_25\Desktop\261111';
pattern    = '*.bmp';
startName  = '';       % 文件名排在它之前的不处理; '' 表示全部处理
forwardDir = 'up';     % 车前进方向在原图中朝哪边: 'up' 'down' 'left' 'right'
tapeW_mm   = 28.8;     % 磁条实际宽度 (mm)，请用卡尺量准，毫米换算全靠它
mmPerPx    = 0;        % >0 时直接用这个标定比例 (原图分辨率)，不再用磁条宽度换算
sensorDown_mm = 32;    % 磁传感器在原图中心正下方的距离 (mm)
tapeW_px   = 0;        % 磁条在原图中的像素宽度; 0 表示自动估计
DS         = 3;        % 计算时先缩小的倍数，越大越快
LH_frac    = 0.2;      % 沿磁条方向只看传感器前后各 (原图高度 × 该比例)
RN_frac    = 0.5;      % 横向搜索范围 ± (原图宽度 × 该比例)
marg_frac  = 0.025;    % 判断边缘外侧时取的条带宽度 (原图宽度 × 该比例)
satLevel   = 250;      % R、G 都不低于此值视为饱和
satWin     = 15;       % 平整度检查窗口 (缩小后像素)
satFrac    = 0.98;     % 窗口内饱和像素比例下限
yelTh      = 25;       % 黄色: min(R,G) - B 下限
yelRatio   = 0.3;      % 黄色: (min(R,G) - B) / max(R,G,B) 下限；按比例判断，阴影里的暗黄磁条也能认出
yelGR      = 0.8;      % 黄色: G >= yelGR * R，排除偏红的棕色泥土
paleRatio  = -0.2;     % 淡黄: (min(R,G) - B) / max(R,G,B) 下限
paleBright = 25;       % 淡黄: 亮度比全图中位数至少高这么多
minScore   = 0.3;      % 带内比例 - 两侧比例均值，低于此值判为无效
dt         = 0.1;      % 文件名里解析不出时间时才使用
gapBreak_s = 3;        % 相邻两帧间隔超过这么多秒，曲线断开
xRefMode   = 0;        % 1: 扣掉参考图的偏差
refFile    = 'ref.bmp';
debugIdx   = [1];      % 显示哪些帧的诊断图，可以写多个
saveOverlay = false;   % true: 每帧叠加结果另存到 folder\overlay

%% ===== 文件列表 =====
files = dir(fullfile(folder, pattern));
files = files(~strcmpi({files.name}, refFile));
if isempty(files)
    error('在 %s 中没有找到匹配 %s 的文件', folder, pattern);
end
[~, idx] = sort({files.name});
files = files(idx);
names = {files.name}';
if ~isempty(startName)
    names = names(cellfun(@(n) ~issorted({n, startName}) || strcmp(n, startName), names));
end
if isempty(names)
    error('startName = %s 之后没有图片', startName);
end
N = numel(names);

I0 = orient(readRGB(fullfile(folder, names{1})), forwardDir);
[H0, W0, ~] = size(I0);
opt = struct('DS', DS, 'LH', LH_frac * H0 / DS, 'RN', RN_frac * W0 / DS, ...
    'marg', max(2, round(marg_frac * W0 / DS)), ...
    'satLevel', satLevel, 'satWin', satWin, 'satFrac', satFrac, ...
    'yelTh', yelTh, 'yelRatio', yelRatio, 'yelGR', yelGR, 'paleRatio', paleRatio, 'paleBright', paleBright);
fprintf('图像 %d x %d，共 %d 帧\n', W0, H0, N);

%% ===== 磁条像素宽度 =====
if tapeW_px > 0
    wPx = tapeW_px;
else
    % 抽约 20 帧，在图像中心附近大范围搜索宽度，取接近竖直、拟合良好的帧的中位数
    optW = opt;
    optW.widths = round(0.08 * W0 / DS) : 3 : round(0.30 * W0 / DS);
    pick = unique(round(linspace(1, N, min(N, 20))));
    ws = [];
    for i = pick
        I = orient(readRGB(fullfile(folder, names{i})), forwardDir);
        Is = imresize(I, 1 / DS, 'box');
        C = candidateMask(Is, opt);
        b = bandFit(C, size(C, 2) / 2, size(C, 1) / 2, optW);
        if b.score > 0.6 && abs(b.th) < 30
            ws(end+1) = b.w; %#ok<AGROW>
        end
    end
    if isempty(ws)
        error('抽样帧中没有拟合出磁条，无法估计宽度。请手动填写 tapeW_px。');
    end
    wPx = median(ws) * DS;
end
opt.widths = unique(round(wPx / DS * (0.92:0.04:1.08)));

if mmPerPx > 0
    s = mmPerPx;
else
    s = tapeW_mm / wPx;
end
sensorDown_px = sensorDown_mm / s;
fprintf('磁条宽度 %.0f 像素，比例 %.4f mm/像素，传感器在中心下方 %.0f 像素 (%.0f mm)\n', ...
    wPx, s, sensorDown_px, sensorDown_mm);

t = parseTimes(names, dt);
if N > 1
    fprintf('时间跨度 %.1f s，相邻帧间隔中位数 %.2f s\n', t(end) - t(1), median(diff(t)));
end
if saveOverlay
    ovDir = fullfile(folder, 'overlay');
    if ~exist(ovDir, 'dir'), mkdir(ovDir); end
end

%% ===== 参考零点 =====
eRefPx = 0;
if xRefMode == 1
    [b0, ~] = processOne(fullfile(folder, refFile), forwardDir, sensorDown_px, opt);
    eRefPx = b0.ePx;
    fprintf('参考图零点: %.1f 像素 (%.2f mm)\n', eRefPx, eRefPx * s);
end

%% ===== 逐张处理 =====
e_px  = nan(N,1); psi = nan(N,1); score = nan(N,1);
wBest = nan(N,1); fIn = nan(N,1); fL = nan(N,1); fR = nan(N,1);
valid = false(N,1);
reason = repmat({''}, N, 1);
edgeUsed = repmat({''}, N, 1);

for i = 1:N
    [b, viz] = processOne(fullfile(folder, names{i}), forwardDir, sensorDown_px, opt);
    score(i) = b.score; wBest(i) = b.w * DS; edgeUsed{i} = b.edge;
    fIn(i) = b.fin; fL(i) = b.fl; fR(i) = b.fr;
    % 方向始终取朝车头的一侧（车与磁条夹角不会超过 90°），不做帧间翻转
    e_px(i) = b.ePx - eRefPx;
    psi(i)  = -b.th;

    if b.score >= minScore
        valid(i) = true;
        reason{i} = 'OK';
    else
        reason{i} = 'LOW_SCORE';
    end

    if ismember(i, debugIdx) || saveOverlay
        fig = showDebug(viz, b, names{i}, e_px(i) * s, psi(i), ismember(i, debugIdx));
        if saveOverlay
            exportgraphics(fig, fullfile(ovDir, [erase(names{i}, '.bmp') '.png']), 'Resolution', 60);
            if ~ismember(i, debugIdx), close(fig); end
        end
    end
end

e_mm = e_px * s;

%% ===== 诊断表 =====
diagT = table(names, round(t - t(1), 2), round(score, 2), round(fIn, 2), round(fL, 2), round(fR, 2), ...
    wBest, edgeUsed, round(psi, 1), round(e_mm, 2), reason, ...
    'VariableNames', {'File','t_s','score','inFrac','leftOut','rightOut','w_px','edge','psi','e_mm','Reason'});
disp(diagT);
fprintf('得分最低 %.2f（%s）\n', min(score), names{find(score == min(score), 1)});
fprintf('边缘精修: 两边 %d 帧, 仅左边 %d, 仅右边 %d, 未精修 %d\n', nnz(strcmp(edgeUsed, 'both')), ...
    nnz(strcmp(edgeUsed, 'left')), nnz(strcmp(edgeUsed, 'right')), nnz(ismember(edgeUsed, {'none', 'rejected'})));

%% ===== 输出 CSV =====
T = table((1:N)', t - t(1), e_mm, psi, valid, score, names, reason, ...
    'VariableNames', {'Index','Time_s','e_perp_mm','psi_deg','valid','score','FileName','Reason'});
outFile = fullfile(folder, 'result_matlab.csv');
writetable(T, outFile);
fprintf('结果已保存: %s\n', outFile);

%% ===== 统计指标 =====
ev = e_mm(valid);
fprintf('有效帧 %d/%d\n', numel(ev), N);
if ~isempty(ev)
    fprintf('e_perp: mean = %.3f, RMS = %.3f, std = %.3f, max|e| = %.3f, P95 = %.3f (mm)\n', ...
        mean(ev), sqrt(mean(ev.^2)), std(ev), max(abs(ev)), prctile95(abs(ev)));
    fprintf('psi   : mean = %.3f, RMS = %.3f (deg)\n', mean(psi(valid)), sqrt(mean(psi(valid).^2)));

    tt = t - t(1);
    eP = e_mm; eP(~valid) = NaN;
    ps = psi;  ps(~valid) = NaN;
    brk = find(diff(tt) > gapBreak_s);
    for k = numel(brk):-1:1
        j = brk(k);
        tt = [tt(1:j); NaN; tt(j+1:end)];
        eP = [eP(1:j); NaN; eP(j+1:end)];
        ps = [ps(1:j); NaN; ps(j+1:end)];
    end
    figure('Name', '偏差曲线');
    subplot(2,1,1); plot(tt, eP, '.-'); grid on; hold on;
    plot(t(~valid) - t(1), zeros(nnz(~valid), 1), 'rx');
    ylabel('e\_perp (mm)'); title('磁传感器到磁条中心线的垂直距离');
    subplot(2,1,2); plot(tt, ps, '.-'); grid on;
    xlabel('时间 (s)'); ylabel('\psi (deg)'); title('航向偏差');
end

%% ===== 函数 =====
function t = parseTimes(names, dt)
    % 文件名形如 HHMMSS.bmp、HHMMSSff.bmp（ff 为百分之一秒）、HHMMSS_fff.bmp 或 HHMMSS.fff.bmp
    N = numel(names);
    t = nan(N, 1);
    for i = 1:N
        tok = regexp(names{i}, '^(\d{2})(\d{2})(\d{2})(?:[._]?(\d{1,3}))?', 'tokens', 'once');
        if isempty(tok), continue; end
        ms = 0;
        if numel(tok) >= 4 && ~isempty(tok{4})
            ms = str2double(tok{4}) / 10^numel(tok{4});
        end
        t(i) = str2double(tok{1}) * 3600 + str2double(tok{2}) * 60 + str2double(tok{3}) + ms;
    end
    if any(isnan(t)) || any(diff(t) < 0)
        t = (0:N-1)' * dt;
    end
end

function [b, viz] = processOne(p, forwardDir, sensorDown_px, opt)
    I = orient(readRGB(p), forwardDir);
    [H0, W0, ~] = size(I);
    [yS0, xS0] = sensorPoint(H0, W0, forwardDir, sensorDown_px);
    Is = imresize(I, 1 / opt.DS, 'box');
    C = candidateMask(Is, opt);
    xS = xS0 / opt.DS; yS = yS0 / opt.DS;
    b = bandFit(C, xS, yS, opt);
    b.ePx = -b.c * opt.DS;
    viz = struct('I', Is, 'C', C, 'xS', xS, 'yS', yS);
end

function I = readRGB(p)
    [I, map] = imread(p);
    if ~isempty(map)
        I = ind2rgb(I, map) * 255;
    end
    if size(I, 3) < 3
        error('%s 不是彩色图像', p);
    end
    I = double(I(:,:,1:3));
end

function I = orient(I, forwardDir)
    switch lower(forwardDir)
        case 'up'
        case 'down',  I = rot90(I, 2);
        case 'left',  I = rot90(I, -1);
        case 'right', I = rot90(I, 1);
        otherwise, error('forwardDir 只能是 up/down/left/right');
    end
end

function [yS, xS] = sensorPoint(H, W, forwardDir, dyPx)
    % dyPx 是原图里“中心点向下”的像素数。这里给的 H、W 已经是车头朝上之后的尺寸。
    yC = H / 2;
    xC = W / 2;
    switch lower(forwardDir)
        case 'up',    yS = yC + dyPx; xS = xC;
        case 'down',  yS = yC - dyPx; xS = xC;
        case 'left',  yS = yC;        xS = xC - dyPx;
        case 'right', yS = yC;        xS = xC + dyPx;
    end
end

function C = candidateMask(I, opt)
    R = I(:,:,1); G = I(:,:,2); B = I(:,:,3);
    RG = min(R, G);
    mx = max(max(R, G), B);
    sat = RG >= opt.satLevel;
    k = opt.satWin;
    satU = conv2(double(sat), ones(k) / k^2, 'same') >= opt.satFrac;
    yel  = (RG - B >= opt.yelTh) & ((RG - B) ./ max(mx, 1) >= opt.yelRatio) & (G >= opt.yelGR * R);
    pale = ((RG - B) ./ max(mx, 1) >= opt.paleRatio) & (mx >= median(mx(:)) + opt.paleBright);
    C = satU | yel | pale;
    % 3x3 开运算去掉零星点
    C = conv2(double(C), ones(3), 'same') >= 9;
    C = conv2(double(C), ones(3), 'same') >= 1;
end

function best = bandFit(C, xS, yS, opt)
    [H, W] = size(C);
    [xx, yy] = meshgrid(0:W-1, 0:H-1);
    px = xx(:) - xS; py = yy(:) - yS; v = double(C(:));
    best = bandSearch(px, py, v, -87:3:90, opt);
    fine = bandSearch(px, py, v, best.th + (-3:0.5:3), opt);
    if fine.score >= best.score
        best = fine;
    end
    best = refineEdges(px, py, v, best, opt);
    best.d  = [sind(best.th), -cosd(best.th)];
    best.nR = [cosd(best.th),  sind(best.th)];
end

function b = refineEdges(px, py, v, b, opt)
    % 固定宽度的直带会为避开候选像素的局部缺块而整体偏斜。
    % 这里沿磁条切成 K 片，每片找左右边缘，两条边各自稳健拟合直线，中心线取两边中间；
    % 一侧整体不可信时用另一侧加减半宽。修正角度超过 maxRot 时保留粗拟合结果。
    K = 12; maxRot = 8;
    th = b.th; w = b.w; c = b.c;
    d = [sind(th), -cosd(th)]; n = [cosd(th), sind(th)];
    s = px * d(1) + py * d(2);
    t = px * n(1) + py * n(2) - c;
    sel = abs(s) <= opt.LH & abs(t) <= w;
    s = s(sel); t = t(sel); vv = v(sel);
    es = linspace(-opt.LH, opt.LH, K + 1);
    nb = floor(2 * w) + 1;
    tc = (0:nb-1)' - w + 0.5;
    Ls = []; Lt = []; Rs = []; Rt = [];
    for k = 1:K
        m = s >= es(k) & s < es(k+1);
        if nnz(m) < 0.5 * (es(2) - es(1)) * w, continue; end
        bi = floor(t(m) + w) + 1;
        ok = bi >= 1 & bi <= nb;
        vm = vv(m);
        cnt = accumarray(bi(ok), 1, [nb 1]);
        on  = accumarray(bi(ok), vm(ok), [nb 1]);
        f = on ./ max(cnt, 1); f(cnt == 0) = NaN;
        g = nan(nb, 1); g(3:nb-2) = f(5:nb) - f(1:nb-4);
        sc = (es(k) + es(k+1)) / 2;
        lw = tc > -0.85 * w & tc < -0.15 * w & ~isnan(g);
        rw = tc >  0.15 * w & tc <  0.85 * w & ~isnan(g);
        if any(lw)
            gg = g; gg(~lw) = -Inf; [mv, j] = max(gg);
            if mv > 0.5, Ls(end+1) = sc; Lt(end+1) = tc(j); end %#ok<AGROW>
        end
        if any(rw)
            gg = g; gg(~rw) = Inf; [mv, j] = min(gg);
            if mv < -0.5, Rs(end+1) = sc; Rt(end+1) = tc(j); end %#ok<AGROW>
        end
    end
    [pL, okL] = robustLine(Ls, Lt);
    [pR, okR] = robustLine(Rs, Rt);
    b.edge = 'none';
    if okL && okR
        mS = (pL(1) + pR(1)) / 2; c0 = (pL(2) + pR(2)) / 2; e = 'both';
    elseif okL
        mS = pL(1); c0 = pL(2) + w / 2; e = 'left';
    elseif okR
        mS = pR(1); c0 = pR(2) - w / 2; e = 'right';
    else
        return;
    end
    if abs(atand(mS)) > maxRot
        b.edge = 'rejected';
        return;
    end
    b.th = th + atand(mS);
    b.c  = (c + c0) / sqrt(1 + mS^2);
    b.edge = e;
end

function [p, ok] = robustLine(s, y)
    p = [0 0]; ok = false;
    s = s(:); y = y(:);
    if numel(s) < 4, return; end
    keep = true(size(s));
    for it = 1:4
        if nnz(keep) < 4, return; end
        p = polyfit(s(keep), y(keep), 1);
        r = y - polyval(p, s);
        sig = 1.4826 * median(abs(r(keep) - median(r(keep))));
        nk = abs(r) <= max(2, 3 * sig);
        if isequal(nk, keep), break; end
        keep = nk;
    end
    ok = nnz(keep) >= max(4, 0.4 * numel(s));
end

function best = bandSearch(px, py, v, ths, opt)
    RN = opt.RN; nb = 2 * ceil(RN); mg = max(1, round(opt.marg));
    best = struct('score', -Inf, 'th', 0, 'c', 0, 'w', opt.widths(1), 'fin', NaN, 'fl', NaN, 'fr', NaN, 'edge', 'none');
    for th = ths
        s = sind(th); c = cosd(th);
        sel = abs(px * s - py * c) <= opt.LH;
        tt = px(sel) * c + py(sel) * s;
        bin = floor(tt + ceil(RN)) + 1;
        ok = bin >= 1 & bin <= nb;
        vs = v(sel);
        cnt = accumarray(bin(ok), 1, [nb 1]);
        on  = accumarray(bin(ok), vs(ok), [nb 1]);
        csC = [0; cumsum(cnt)]; csO = [0; cumsum(on)];
        for w = opt.widths
            k0 = (mg + 1 : nb - w - mg + 1)';
            inC = csC(k0 + w) - csC(k0);          inO = csO(k0 + w) - csO(k0);
            lC  = csC(k0) - csC(k0 - mg);         lO  = csO(k0) - csO(k0 - mg);
            rC  = csC(k0 + w + mg) - csC(k0 + w); rO  = csO(k0 + w + mg) - csO(k0 + w);
            fin = inO ./ max(inC, 1); fl = lO ./ max(lC, 1); fr = rO ./ max(rC, 1);
            sc = fin - 0.5 * (fl + fr);
            sc(inC < 0.3 * w * opt.LH | lC == 0 | rC == 0) = -Inf;
            [m, j] = max(sc);
            if m > best.score
                best.score = m; best.th = th; best.w = w;
                best.c = -ceil(RN) + k0(j) - 1 + w / 2;
                best.fin = fin(j); best.fl = fl(j); best.fr = fr(j);
            end
        end
    end
end

function fig = showDebug(viz, b, name, eMm, psiDeg, visible)
    if visible, vis = 'on'; else, vis = 'off'; end
    fig = figure('Name', ['诊断: ' name], 'Position', [100 100 1200 480], 'Visible', vis);
    S = [viz.xS, viz.yS];
    P0 = S + b.c * b.nR;
    L = 2 * max(size(viz.C));
    for k = 1:2
        subplot(1,2,k);
        if k == 1, imshow(uint8(viz.I)); else, imshow(viz.C); end
        hold on;
        for off = [-b.w / 2, 0, b.w / 2]
            A = P0 + off * b.nR - L * b.d; Bp = P0 + off * b.nR + L * b.d;
            if off == 0, col = 'm-'; else, col = 'c-'; end
            plot([A(1) Bp(1)] + 1, [A(2) Bp(2)] + 1, col, 'LineWidth', 1.5);
        end
        plot([S(1) P0(1)] + 1, [S(2) P0(2)] + 1, 'r-', 'LineWidth', 2);
        plot(S(1) + 1, S(2) + 1, 'g+', 'MarkerSize', 18, 'LineWidth', 2);
        xlim([0.5, size(viz.C, 2) + 0.5]); ylim([0.5, size(viz.C, 1) + 0.5]);
        hold off;
    end
    subplot(1,2,1);
    title(sprintf('%s  e=%.1f mm  \\psi=%.1f°  score=%.2f', name, eMm, psiDeg, b.score), 'Interpreter', 'tex');
    subplot(1,2,2);
    title('候选像素; 青线=磁条两边, 品红=中心线, 红=垂线');
end

function v = prctile95(x)
    if isempty(x), v = NaN; return; end
    x = sort(x);
    v = x(max(1, ceil(0.95 * numel(x))));
end
