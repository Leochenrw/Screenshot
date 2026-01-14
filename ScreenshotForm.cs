using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ScreenshotsNotifier
{
    public class ScreenshotForm : Form
    {
        private readonly string filePath;
        private PictureBox? thumbnailPictureBox;
        private TextBox? pathTextBox;
        private Button? copyButton;
        private Button? closeButton;
        private static int openFormCount = 0;
        private const int MaxOpenForms = 5;

        public ScreenshotForm(string filePath)
        {
            this.filePath = filePath;
            this.InitializeComponent();
            this.LoadScreenshot();
        }

        private void InitializeComponent()
        {
            // 窗体属性
            this.Text = "ScreenshotsNotifier";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(480, 550);
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // 计算窗口位置（右下角）
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            var x = workingArea.Right - this.Width - 20;
            var y = workingArea.Bottom - this.Height - 20 - (openFormCount * 30);

            // 确保窗口在屏幕内
            if (x < 0) x = 0;
            if (y < 0) y = 0;

            this.Location = new Point(x, y);

            // 标题标签
            var titleLabel = new Label
            {
                Text = "🖼️ 新截图已保存",
                Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 34, 34),
                AutoSize = true,
                Location = new Point(24, 20)
            };
            this.Controls.Add(titleLabel);

            // 副标题标签
            var subtitleLabel = new Label
            {
                Text = "您的截图已成功保存到本地",
                Font = new Font("Microsoft YaHei UI", 9F),
                ForeColor = Color.FromArgb(102, 102, 102),
                AutoSize = true,
                Location = new Point(24, 55)
            };
            this.Controls.Add(subtitleLabel);

            // 缩略图容器
            var thumbnailContainer = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Size = new Size(432, 220),
                Location = new Point(24, 90)
            };
            this.Controls.Add(thumbnailContainer);

            // 缩略图
            thumbnailPictureBox = new PictureBox
            {
                Size = new Size(400, 180),
                Location = new Point(16, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            thumbnailContainer.Controls.Add(thumbnailPictureBox);

            // 路径标签
            var pathLabel = new Label
            {
                Text = "📁 文件路径：",
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 34, 34),
                AutoSize = true,
                Location = new Point(24, 330)
            };
            this.Controls.Add(pathLabel);

            // 路径文本框
            pathTextBox = new TextBox
            {
                Text = filePath,
                Font = new Font("Consolas", 9F),
                ForeColor = Color.FromArgb(34, 34, 34),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                Multiline = true,
                Size = new Size(432, 40),
                Location = new Point(24, 355),
                ScrollBars = ScrollBars.None
            };
            this.Controls.Add(pathTextBox);

            // 按钮容器
            var buttonPanel = new Panel
            {
                Size = new Size(432, 40),
                Location = new Point(24, 420)
            };
            this.Controls.Add(buttonPanel);

            // 复制路径按钮
            copyButton = new Button
            {
                Text = "复制路径",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei UI", 9F),
                Cursor = Cursors.Hand,
                Location = new Point(100, 0)
            };
            copyButton.FlatAppearance.BorderSize = 0;
            copyButton.Click += CopyButton_Click;
            copyButton.MouseEnter += (s, e) => copyButton.BackColor = Color.FromArgb(16, 110, 190);
            copyButton.MouseLeave += (s, e) => copyButton.BackColor = Color.FromArgb(0, 120, 212);
            buttonPanel.Controls.Add(copyButton);

            // 关闭按钮
            closeButton = new Button
            {
                Text = "关闭",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(243, 243, 243),
                ForeColor = Color.FromArgb(34, 34, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei UI", 9F),
                Cursor = Cursors.Hand,
                Location = new Point(220, 0)
            };
            closeButton.FlatAppearance.BorderColor = Color.FromArgb(209, 209, 209);
            closeButton.FlatAppearance.BorderSize = 1;
            closeButton.Click += CloseButton_Click;
            closeButton.MouseEnter += (s, e) => closeButton.BackColor = Color.FromArgb(229, 229, 229);
            closeButton.MouseLeave += (s, e) => closeButton.BackColor = Color.FromArgb(243, 243, 243);
            buttonPanel.Controls.Add(closeButton);

            openFormCount++;
        }

        private void LoadScreenshot()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (Image originalImage = Image.FromFile(filePath))
                    {
                        thumbnailPictureBox!.Image = new Bitmap(originalImage);
                    }
                }
                else
                {
                    // 显示占位符
                    thumbnailPictureBox!.Image = CreatePlaceholderImage();
                }
            }
            catch
            {
                thumbnailPictureBox!.Image = CreatePlaceholderImage();
            }
        }

        private Image CreatePlaceholderImage()
        {
            var bitmap = new Bitmap(400, 180);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(221, 221, 221));
                using (var font = new Font("Microsoft YaHei UI", 12))
                using (var brush = new SolidBrush(Color.FromArgb(102, 102, 102)))
                {
                    var text = "无法加载预览";
                    var size = g.MeasureString(text, font);
                    var x = (400 - size.Width) / 2;
                    var y = (180 - size.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            return bitmap;
        }

        private void CopyButton_Click(object? sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(filePath);
                copyButton!.Text = "✓ 已复制";
                copyButton.BackColor = Color.FromArgb(16, 110, 190);

                // 2秒后恢复
                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (s, args) =>
                {
                    copyButton.Text = "复制路径";
                    copyButton.BackColor = Color.FromArgb(0, 120, 212);
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"复制失败: {ex.Message}",
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CloseButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            openFormCount--;
            thumbnailPictureBox?.Image?.Dispose();
            base.OnFormClosing(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // 支持 ESC 键关闭
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            // 支持 Ctrl+C 复制
            if (keyData == (Keys.Control | Keys.C))
            {
                CopyButton_Click(null, EventArgs.Empty);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
