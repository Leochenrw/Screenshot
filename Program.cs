using System;
using System.Windows.Forms;

namespace ScreenshotsNotifier
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 创建主窗体（隐藏）
            var mainForm = new MainForm();
            Application.Run(mainForm);
        }
    }
}
