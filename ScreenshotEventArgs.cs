using System;

namespace ScreenshotsNotifier
{
    public class ScreenshotEventArgs : EventArgs
    {
        public string FilePath { get; }

        public ScreenshotEventArgs(string filePath)
        {
            FilePath = filePath;
        }
    }
}
