# ScreenshotsNotifier 📸

一个简洁实用的 Windows 桌面应用程序，自动监听系统截图并弹窗显示图片存储路径。

![.NET](https://img.shields.io/badge/.NET-6.0-purple)
![Windows](https://img.shields.io/badge/Windows-10%2B-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## ✨ 功能特性

- 📋 **剪贴板监听** - 自动检测 Win+Shift+S 截图
- 🖼️ **自动保存** - 截图自动保存到指定文件夹
- 📢 **智能通知** - 弹窗显示截图预览和文件路径
- 📋 **一键复制** - 快速复制图片路径到剪贴板
- 🔔 **后台运行** - 最小化到系统托盘，不干扰工作
- ⌨️ **快捷键支持** - ESC 关闭，Ctrl+C 复制
- ✂️ **剪刀图标** - 自定义图标，符合截图工具主题

## 🎬 演示

启动程序后，按 `Win + Shift + S` 截图，程序会自动：

1. 捕获剪贴板中的图片
2. 保存到 `Pictures\Screenshots` 文件夹
3. 弹窗显示截图预览和完整路径
4. 提供一键复制路径功能

![界面预览](screenshot-preview.html)

## 🚀 快速开始

### 环境要求

- Windows 10/11 操作系统
- .NET 6.0 Runtime 或更高版本

### 安装运行

**方法 1：下载预编译版本（推荐）**

1. 下载最新版本的 `ScreenshotsNotifier.exe`
2. 双击运行即可

**方法 2：从源码运行**

```bash
# 克隆仓库
git clone https://github.com/Leochenrw/screenshot.git
cd screenshot

# 运行程序
双击 启动程序.bat
# 或使用命令行
dotnet run
```

**方法 3：生成独立可执行文件**

```bash
# 发布为单文件可执行程序
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# 可执行文件位于：bin\Release\net6.0-windows\win-x64\publish\ScreenshotsNotifier.exe
```

## 📁 截图保存位置

默认保存路径：
```
C:\Users\[用户名]\Pictures\Screenshots
```

文件命名格式：
```
Screenshot_YYYYMMDD_HHMMSS.png
```

## 🎯 使用方法

### 基本使用

1. **启动程序**
   - 双击 `启动程序.bat` 或直接运行 `ScreenshotsNotifier.exe`
   - 程序最小化到系统托盘（剪刀图标 ✂️）

2. **截图**
   - 按下 `Win + Shift + S`
   - 选择截图区域
   - 程序自动保存并弹窗通知

3. **复制路径**
   - 在弹窗中点击"复制路径"按钮
   - 或按 `Ctrl + C` 快捷键
   - 路径已复制到剪贴板

4. **退出程序**
   - 右键点击系统托盘图标
   - 选择"退出"

### 快捷键

- `Ctrl + C` - 复制文件路径
- `ESC` - 关闭通知窗口

## 🛠️ 技术栈

- **框架**: .NET 6.0 Windows
- **UI**: Windows Forms
- **语言**: C# 10
- **依赖**: System.Drawing.Common 7.0.0

## 📂 项目结构

```
ScreenshotsNotifier/
├── Program.cs                 # 应用程序入口
├── MainForm.cs               # 主窗体和协调逻辑
├── ClipboardMonitor.cs       # 剪贴板监听器
├── FolderWatcher.cs          # 文件夹监听器（备用）
├── ScreenshotForm.cs         # 截图通知弹窗
├── ScreenshotEventArgs.cs    # 事件参数类
├── ScreenshotsNotifier.csproj # 项目配置
├── 启动程序.bat               # 启动脚本
├── screenshot-preview.html   # 界面预览
├── README.md                 # 项目文档
└── .gitignore               # Git 忽略规则
```

## 🔧 配置

### 修改截图保存路径

编辑 `MainForm.cs` 文件，修改 `screenshotsPath` 变量：

```csharp
screenshotsPath = @"D:\MyScreenshots";  // 自定义路径
```

### 开机自动启动

1. 按 `Win + R`，输入 `shell:startup`
2. 在打开的文件夹中创建程序快捷方式

## 🐛 已知问题

- 截图文件名冲突时会自动添加序号（_1, _2, ...）
- 同时打开多个通知窗口时会自动级联显示

## 📝 开发计划

- [ ] 添加配置界面（自定义保存路径、文件名格式）
- [ ] 支持自定义快捷键
- [ ] 添加截图历史记录查看功能
- [ ] 支持图片格式转换（PNG/JPEG）
- [ ] 添加主题切换功能

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 🙏 致谢

- 感谢所有贡献者
- 感谢 .NET 社区的支持

## 📧 联系方式

- 提交 [Issue](https://github.com/Leochenrw/screenshot/issues)
- Pull Request 欢迎提交

---

**享受高效的截图体验！** 📸✨
