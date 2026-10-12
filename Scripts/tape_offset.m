%% 磁条横向偏差离线计算（黄磁条：min(R,G)-B）
clear; clc; close all;

%% ===== 参数 =====
folder     = 'D:\CarTest\20261001_run01';   % 图片所在文件夹
pattern    = '*.bmp';                       % 文件匹配模式，单张测试可直接写文件名
forwardDir = 'up';     % 车前进方向在原图中朝哪边: 'up' 'down' 'left' 'right'
th         = 100;      % 黄条增强图阈值；你这张图磁条~200、地面噪声~40，40 太低
smoothK    = 5;        % 增强图平滑窗口 (像素), 1 表示不平滑
openK      = 5;        % 开运算核边长(奇数)，去掉孤立白点；1 表示不做
tapeW_mm   = 50;       % 磁条实际宽度 (mm)
mmPerPx    = 0;        % 标定得到的 mm/像素; 0 表示用磁条宽度自动估计
dt         = 0.1;      % 采样间隔 (s)
winHalf    = 80;       % 局部窗口半高 (行)
fitOrder   = 1;        % 1: 直线, 2: 二次曲线
minRatio   = 0.7;      % 窗口内有效行最低比例
maxWStd    = 3;        % 窗口内宽度标准差上限 (像素)
widthTol   = 0.2;      % 宽度相对中位数的允许偏差
xRefMode   = 0;        % 0: 零点取图像中心列; 1: 用参考图
refFile    = 'ref.bmp';
debugIdx   = 1;        % 显示第几张图的诊断图, 0 表示不显示

%% ===== 文件列表 =====
files = dir(fullfile(folder, pattern));
files = files(~strcmpi({files.name}, refFile));
if isempty(files)
    error('在 %s 中没有找到匹配 %s 的文件', folder, pattern);
end
[~, idx] = sort({files.name});
files = files(idx);
names = {files.name}';
N = numel(names);

opt = struct('th', th, 'smoothK', smoothK, 'openK', openK, ...
             'winHalf', winHalf, 'fitOrder', fitOrder);

%% ===== 参考零点 =====
xRef = NaN;
if xRefMode == 1
    r0 = measureLocal(orient(readRGB(fullfile(folder, refFile)), forwardDir), opt);
    if isnan(r0.xt)
        error('参考图 %s 中未检测到磁条', refFile);
    end
    xRef = r0.xt;
end

%% ===== 逐张处理 =====
xt = nan(N,1); xUse = nan(N,1); aArr = nan(N,1);
wp = nan(N,1); wStd = nan(N,1);
nValid = zeros(N,1); nWin = zeros(N,1);
valid  = false(N,1);
reason = repmat({''}, N, 1);

for i = 1:N
    I = orient(readRGB(fullfile(folder, names{i})), forwardDir);
    r = measureLocal(I, opt);
    nValid(i) = r.nValid; nWin(i) = r.nWin; wStd(i) = r.wStd; xt(i) = r.xt;

    if xRefMode == 0, xUse(i) = r.W / 2; else, xUse(i) = xRef; end

    if i == debugIdx
        showDebug(I, r, opt, xUse(i), names{i});
    end

    if isnan(r.xt)
        reason{i} = 'FEW_POINTS';
        continue;
    end

    aArr(i) = r.a;
    wp(i)   = r.wMean / sqrt(1 + r.a^2);

    if r.nValid < minRatio * r.nWin
        reason{i} = 'LOW_RATIO';
    elseif r.wStd >= maxWStd
        reason{i} = 'WIDTH_STD';
    else
        valid(i) = true;
    end
end

if any(valid)
    wpMed = median(wp(valid));
    bad = valid & abs(wp - wpMed) >= widthTol * wpMed;
    valid(bad)  = false;
    reason(bad) = {'WIDTH_DEV'};
end
reason(valid) = {'OK'};

%% ===== 毫米换算 =====
if mmPerPx > 0
    s = mmPerPx;
    fprintf('使用标定比例: %.4f mm/像素\n', s);
elseif any(valid)
    s = tapeW_mm / median(wp(valid));
    fprintf('由磁条宽度估计比例: %.4f mm/像素 (磁条约 %.1f 像素宽)\n', s, median(wp(valid)));
else
    s = NaN;
end

k      = sqrt(1 + aArr.^2);
e_lat  = (xUse - xt) * s;
e_perp = e_lat ./ k;
psi    = atand(aArr);

%% ===== 诊断表 =====
diagT = table(names, nValid, nWin, round(wStd,2), round(xt,1), round(wp,1), ...
    round(e_lat,3), reason, ...
    'VariableNames', {'File','nValid','nWin','wStd','xt','wp','e_lat','Reason'});
disp(diagT);

%% ===== 输出 CSV =====
t = (0:N-1)' * dt;
T = table((1:N)', t, e_lat, e_perp, psi, valid, nValid, wStd, wp, names, reason, ...
    'VariableNames', {'Index','Time_s','e_lat_mm','e_perp_mm','psi_deg','valid', ...
                      'nValid','wStd_px','wp_px','FileName','Reason'});
outFile = fullfile(folder, 'result_matlab.csv');
writetable(T, outFile);
fprintf('结果已保存: %s\n', outFile);

%% ===== 统计指标 =====
ev = e_lat(valid);
fprintf('有效帧 %d/%d\n', numel(ev), N);
if isempty(ev)
    fprintf('没有有效帧，请根据诊断表的 Reason 列和诊断图调整参数。\n');
else
    fprintf('e_lat : mean = %.3f, RMS = %.3f, std = %.3f, max|e| = %.3f, P95 = %.3f (mm)\n', ...
        mean(ev), sqrt(mean(ev.^2)), std(ev), max(abs(ev)), prctile95(abs(ev)));
    fprintf('psi   : mean = %.3f, RMS = %.3f (deg)\n', ...
        mean(psi(valid)), sqrt(mean(psi(valid).^2)));
    if numel(ev) > 1
        figure('Name', '偏差曲线');
        subplot(2,1,1); plot(t(valid), ev, '.-'); grid on;
        ylabel('e\_lat (mm)'); title('车头中心横向偏差');
        subplot(2,1,2); plot(t(valid), psi(valid), '.-'); grid on;
        xlabel('时间 (s)'); ylabel('\psi (deg)'); title('航向偏差');
    end
end

%% ===== 函数 =====
function I = readRGB(p)
    [I, map] = imread(p);
    if ~isempty(map)
        I = ind2rgb(I, map) * 255;
    end
    if size(I, 3) < 3
        error('%s 不是彩色图像，无法做颜色分割', p);
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

function [B, D] = segment(I, opt)
    % 黄磁条：R、G 都高，B 低。用 min(R,G)-B，比单纯 R-B 更能压掉偏红的地面划痕
    D = min(I(:,:,1), I(:,:,2)) - I(:,:,3);
    if opt.smoothK > 1
        D = conv2(D, ones(opt.smoothK) / opt.smoothK^2, 'same');
    end
    B = D >= opt.th;
    % 3x3 多数滤波
    B = conv2(double(B), ones(3), 'same') >= 5;
    % 开运算：先腐蚀再膨胀，去掉孤立白点和细碎噪声
    k = opt.openK;
    if k > 1
        if mod(k, 2) == 0, k = k + 1; end
        ker = ones(k);
        B = conv2(double(B), ker, 'same') >= numel(ker);           % 腐蚀
        B = conv2(double(B), ker, 'same') >= 1;                    % 膨胀
    end
end

function r = measureLocal(I, opt)
    B = segment(I, opt);
    [H, W] = size(B);
    yRef = H / 2;
    winHalf = opt.winHalf;

    y0 = max(0, ceil(yRef - winHalf));
    y1 = min(H - 1, floor(yRef + winHalf));
    rows = (y0:y1)';
    xL = nan(size(rows)); xR = nan(size(rows));
    for j = 1:numel(rows)
        [xL(j), xR(j)] = longestRun(B(rows(j) + 1, :));
    end
    ok = ~isnan(xL);
    y  = rows(ok);
    xL = xL(ok) - 1;
    xR = xR(ok) - 1;
    xc = (xL + xR) / 2;
    w  = xR - xL + 1;

    if ~isempty(w)
        wm  = median(w);
        okw = abs(w - wm) <= 0.5 * wm;
        y = y(okw); xc = xc(okw); w = w(okw);
    end

    r.H = H; r.W = W; r.yRef = yRef;
    r.yAll = y; r.xcAll = xc;
    r.nWin   = numel(rows);
    r.nValid = numel(y);
    r.xt = NaN; r.a = NaN; r.wMean = NaN; r.wStd = NaN; r.p = [];
    if r.nValid < max(10, opt.fitOrder + 5), return; end

    u = y - yRef;
    keep = abs(xc - median(xc)) <= 3 * max(1, 1.4826 * median(abs(xc - median(xc))));
    if nnz(keep) < opt.fitOrder + 5, keep = true(size(u)); end
    for it = 1:5
        p = polyfit(u(keep), xc(keep), opt.fitOrder);
        res = xc - polyval(p, u);
        sig = 1.4826 * median(abs(res(keep)));
        newKeep = abs(res) <= max(2, 3 * sig);
        if nnz(newKeep) < opt.fitOrder + 5 || isequal(newKeep, keep), break; end
        keep = newKeep;
    end

    r.p  = p;
    r.xt = p(end);
    r.a  = p(end-1);
    r.yAll = y(keep); r.xcAll = xc(keep);
    r.wMean = mean(w(keep));
    r.wStd  = std(w(keep));
end

function [xL, xR] = longestRun(row)
    d  = diff([false, row, false]);
    st = find(d == 1);
    en = find(d == -1) - 1;
    if isempty(st)
        xL = NaN; xR = NaN; return;
    end
    [~, k] = max(en - st);
    xL = st(k); xR = en(k);
end

function showDebug(I, r, opt, xUse, name)
    [B, D] = segment(I, opt);
    winHalf = opt.winHalf;
    figure('Name', ['诊断: ' name], 'Position', [100 100 1400 500]);

    subplot(1,3,1); imshow(uint8(I)); hold on;
    plot([1 r.W], [r.yRef-winHalf r.yRef-winHalf] + 1, 'y--', 'LineWidth', 1);
    plot([1 r.W], [r.yRef+winHalf r.yRef+winHalf] + 1, 'y--', 'LineWidth', 1);
    plot(r.xcAll + 1, r.yAll + 1, 'c.', 'MarkerSize', 4);
    if ~isnan(r.xt)
        uu = linspace(-winHalf, winHalf, 50);
        plot(polyval(r.p, uu) + 1, uu + r.yRef + 1, 'm-', 'LineWidth', 1.5);
        plot(r.xt + 1, r.yRef + 1, 'mo', 'MarkerSize', 10, 'LineWidth', 2);
    end
    plot(xUse + 1, r.yRef + 1, 'g+', 'MarkerSize', 20, 'LineWidth', 2);
    hold off;
    title({'车头朝上: 黄虚线=窗口, 青点=参与拟合的中心点', ...
           '品红线=拟合, 品红圈=磁条中心, 绿十字=零点'});

    subplot(1,3,2); imshow(D, []); colorbar; title('min(R,G)-B (平滑后)');
    subplot(1,3,3); imshow(B); title(sprintf('二值图 (th = %g, open=%g)', opt.th, opt.openK));

    if any(B(:)) && any(~B(:))
        fprintf('[诊断] %s: 增强图中位数  磁条 = %.1f, 背景 = %.1f, 当前 th = %g\n', ...
            name, median(D(B)), median(D(~B)), opt.th);
    else
        fprintf('[诊断] %s: 二值图全白或全黑, 增强图范围 [%.1f, %.1f]\n', ...
            name, min(D(:)), max(D(:)));
    end
end

function v = prctile95(x)
    if isempty(x), v = NaN; return; end
    x = sort(x);
    v = x(max(1, ceil(0.95 * numel(x))));
end
