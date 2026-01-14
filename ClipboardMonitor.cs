using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenshotsNotifier
{
    public class ClipboardMonitor : NativeWindow, IDisposable
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private readonly string screenshotsPath;
        private string? lastClipboardHash;

        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        public event EventHandler<ScreenshotEventArgs>? ScreenshotCaptured;

        public ClipboardMonitor(string screenshotsPath)
        {
            this.screenshotsPath = screenshotsPath;
            this.CreateHandle(new CreateParams { Parent = IntPtr.Zero });

            if (!AddClipboardFormatListener(this.Handle))
            {
                throw new InvalidOperationException("无法注册剪贴板监听器");
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                HandleClipboardUpdate();
            }
        }

        private void HandleClipboardUpdate()
        {
            try
            {
                if (!Clipboard.ContainsImage())
                {
                    return;
                }

                // 获取剪贴板中的图片
                var image = Clipboard.GetImage();
                if (image == null)
                {
                    return;
                }

                // 生成哈希以检测重复
                var currentHash = ComputeImageHash(image);
                if (currentHash == lastClipboardHash)
                {
                    // 同一张图片，忽略
                    return;
                }
                lastClipboardHash = currentHash;

                // 保存图片
                var filePath = SaveScreenshot(image);
                if (filePath != null)
                {
                    ScreenshotCaptured?.Invoke(this, new ScreenshotEventArgs(filePath));
                }
            }
            catch (Exception ex)
            {
                // 静默处理错误，避免频繁弹窗
                Debug.WriteLine($"剪贴板监听错误: {ex.Message}");
            }
        }

        private string? ComputeImageHash(Image image)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    var hash = ms.ToArray().Take(100).ToArray(); // 取前100字节作为简单哈希
                    return Convert.ToBase64String(hash);
                }
            }
            catch
            {
                return null;
            }
        }

        private string? SaveScreenshot(Image image)
        {
            try
            {
                // 生成唯一文件名
                var fileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var filePath = Path.Combine(screenshotsPath, fileName);

                // 如果文件已存在，添加序号
                int counter = 1;
                while (File.Exists(filePath))
                {
                    fileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}_{counter}.png";
                    filePath = Path.Combine(screenshotsPath, fileName);
                    counter++;
                }

                // 保存图片
                image.Save(filePath, ImageFormat.Png);
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"保存截图失败: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                if (this.Handle != IntPtr.Zero)
                {
                    RemoveClipboardFormatListener(this.Handle);
                }
            }
            catch
            {
                // 忽略错误
            }
        }
    }
}
