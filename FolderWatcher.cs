using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ScreenshotsNotifier
{
    public class FolderWatcher : IDisposable
    {
        private readonly FileSystemWatcher watcher;
        private readonly ConcurrentQueue<string> recentFiles;
        private readonly string screenshotsPath;
        private readonly string[] imageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
        private System.Timers.Timer? cleanupTimer;

        public event EventHandler<ScreenshotEventArgs>? ScreenshotCreated;

        public FolderWatcher(string screenshotsPath)
        {
            this.screenshotsPath = screenshotsPath;
            this.recentFiles = new ConcurrentQueue<string>();

            watcher = new FileSystemWatcher(screenshotsPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                EnableRaisingEvents = true,
                InternalBufferSize = 64 * 1024 // 64KB 缓冲区
            };

            watcher.Created += OnFileCreated;
            watcher.Error += OnError;

            // 启动清理任务
            StartCleanupTask();
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                var filePath = e.FullPath;

                // 检查文件扩展名
                var extension = Path.GetExtension(filePath).ToLower();
                if (!imageExtensions.Contains(extension))
                {
                    return;
                }

                // 检查是否最近已处理
                if (IsRecentlyProcessed(filePath))
                {
                    return;
                }

                // 等待文件完全写入
                WaitForFileReady(filePath);

                // 标记为已处理
                recentFiles.Enqueue(filePath);

                // 触发事件
                ScreenshotCreated?.Invoke(this, new ScreenshotEventArgs(filePath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"文件夹监听错误: {ex.Message}");
            }
        }

        private bool IsRecentlyProcessed(string filePath)
        {
            // 检查队列中是否有此文件（5秒内的重复）
            var cutoffTime = DateTime.Now.AddSeconds(-5);
            var tempQueue = new ConcurrentQueue<string>();

            bool found = false;
            while (recentFiles.TryDequeue(out string? file))
            {
                if (file == filePath && File.GetCreationTime(file) > cutoffTime)
                {
                    found = true;
                }
                tempQueue.Enqueue(file);
            }

            // 重新入队
            while (tempQueue.TryDequeue(out string? file))
            {
                recentFiles.Enqueue(file);
            }

            return found;
        }

        private void WaitForFileReady(string filePath)
        {
            const int maxRetries = 10;
            const int delayMs = 100;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // 尝试打开文件以确保其可访问
                    using (var stream = File.OpenRead(filePath))
                    {
                        return; // 文件就绪
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
        }

        private void StartCleanupTask()
        {
            // 定期清理超过5秒的记录
            cleanupTimer = new System.Timers.Timer(5000);
            cleanupTimer.Elapsed += (s, e) =>
            {
                var cutoffTime = DateTime.Now.AddSeconds(-5);
                var tempQueue = new ConcurrentQueue<string>();

                while (recentFiles.TryDequeue(out string? file))
                {
                    if (File.Exists(file) && File.GetCreationTime(file) > cutoffTime)
                    {
                        tempQueue.Enqueue(file);
                    }
                }

                while (tempQueue.TryDequeue(out string? file))
                {
                    recentFiles.Enqueue(file);
                }
            };
            cleanupTimer.Start();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine($"FileSystemWatcher错误: {e.GetException().Message}");

            // 尝试重新启动监听
            try
            {
                watcher.EnableRaisingEvents = false;

                if (!Directory.Exists(screenshotsPath))
                {
                    Directory.CreateDirectory(screenshotsPath);
                }

                watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"重启监听器失败: {ex.Message}");
            }
        }

        public void Dispose()
        {
            cleanupTimer?.Stop();
            cleanupTimer?.Dispose();
            watcher?.Dispose();
        }
    }
}
