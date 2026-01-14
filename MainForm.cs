using System;
using System.IO;
using System.Windows.Forms;

namespace ScreenshotsNotifier
{
    public class MainForm : Form
    {
        private ClipboardMonitor? clipboardMonitor;
        private FolderWatcher? folderWatcher;
        private NotifyIcon? trayIcon;
        private string screenshotsPath;

        public MainForm()
        {
            // 设置窗体属性（隐藏主窗口）
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;

            // 初始化截图保存路径
            screenshotsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "Screenshots"
            );

            // 确保截图文件夹存在
            if (!Directory.Exists(screenshotsPath))
            {
                Directory.CreateDirectory(screenshotsPath);
            }

            // 初始化组件
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // 初始化剪贴板监听器（主要监听方式）
            clipboardMonitor = new ClipboardMonitor(screenshotsPath);
            clipboardMonitor.ScreenshotCaptured += OnScreenshotCaptured;

            // 暂时禁用文件夹监听器，避免重复触发
            // folderWatcher = new FolderWatcher(screenshotsPath);
            // folderWatcher.ScreenshotCreated += OnScreenshotCaptured;

            // 初始化系统托盘
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            // 创建剪刀图标
            var icon = CreateScissorsIcon();

            trayIcon = new NotifyIcon
            {
                Text = "ScreenshotsNotifier - 截图通知工具",
                Icon = icon,
                Visible = true
            };

            // 创建右键菜单
            var contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("退出");
            exitItem.Click += (s, e) => ExitApplication();
            contextMenu.Items.Add(exitItem);

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.DoubleClick += (s, e) =>
            {
                // 双击显示关于信息（可选）
                MessageBox.Show(
                    "ScreenshotsNotifier v1.0\n\n监听剪贴板中的截图\n并在保存后弹窗显示路径",
                    "关于",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };
        }

        private Icon CreateScissorsIcon()
        {
            // 创建一个简单的位图并绘制剪刀
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 绘制剪刀
                using (var pen = new Pen(Color.FromArgb(0, 120, 212), 2))
                {
                    // 左刀片
                    g.DrawLine(pen, 4, 6, 6, 10);
                    g.DrawLine(pen, 6, 10, 8, 6);

                    // 右刀片
                    g.DrawLine(pen, 8, 6, 10, 10);
                    g.DrawLine(pen, 10, 10, 12, 6);
                }

                // 刀柄
                using (var handlePen = new Pen(Color.FromArgb(100, 100, 100), 2))
                {
                    g.DrawLine(handlePen, 5, 6, 5, 3);
                    g.DrawLine(handlePen, 11, 6, 11, 3);
                }
            }

            // 从位图创建图标句柄
            var hIcon = bitmap.GetHicon();
            return Icon.FromHandle(hIcon);
        }

        private void OnScreenshotCaptured(object? sender, ScreenshotEventArgs e)
        {
            // 在UI线程中显示弹窗
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object?, ScreenshotEventArgs>(OnScreenshotCaptured), sender, e);
                return;
            }

            try
            {
                var form = new ScreenshotForm(e.FilePath);
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"显示截图通知失败：{ex.Message}",
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private bool isExiting = false;

        private void ExitApplication()
        {
            if (isExiting) return;
            isExiting = true;

            try
            {
                // 取消事件订阅，防止在Dispose时触发事件
                if (clipboardMonitor != null)
                {
                    clipboardMonitor.ScreenshotCaptured -= OnScreenshotCaptured;
                }
                // folderWatcher 已禁用
                // if (folderWatcher != null)
                // {
                //     folderWatcher.ScreenshotCreated -= OnScreenshotCaptured;
                // }

                // 释放资源
                clipboardMonitor?.Dispose();
                // folderWatcher?.Dispose();

                // 隐藏托盘图标
                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                }

                // 强制退出
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"退出时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 如果正在退出，允许关闭
            if (isExiting)
            {
                base.OnFormClosing(e);
                return;
            }

            // 防止窗体关闭，改为最小化到托盘
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            base.OnFormClosing(e);
        }
    }
}
