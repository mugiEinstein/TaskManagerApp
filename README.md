# TaskManagerApp

TaskManagerApp 是一个基于 **C#** 和 **XAML** 构建的桌面任务管理应用程序。项目采用 **MVVM (Model-View-ViewModel)** 架构模式设计，旨在提供一个清晰、可维护且功能丰富的任务管理解决方案。

## 📋 项目简介

本项目展示了如何使用现代 .NET 技术栈开发桌面应用。它实现了任务的创建、读取、更新和删除 (CRUD) 操作，并通过分层架构将 UI 逻辑与业务逻辑分离，确保代码的高内聚和低耦合。

## 🚀 技术栈

- **开发语言**: C#
- **UI 框架**: WPF (Windows Presentation Foundation) / XAML
- **架构模式**: MVVM (Model-View-ViewModel)
- **数据访问**: Entity Framework Core (推测，基于 `Migrations` 目录)
- **设计模式**: Repository Pattern (仓储模式)
- **开发工具**: Visual Studio

## 📂 项目结构

项目结构遵循清晰的分层设计：

```text
TaskManagerApp/
├── 📁 Converters      # XAML 数据转换器 (IValueConverter 实现)
├── 📁 Data            # 数据库上下文 (DbContext) 及相关配置
├── 📁 Infrastructure  # 基础设施代码，如依赖注入配置或通用工具
├── 📁 Migrations      # Entity Framework 数据库迁移文件
├── 📁 Models          # 领域实体模型 (数据对象)
├── 📁 Repositories    # 数据访问层，封装数据库操作
├── 📁 Services        # 业务逻辑服务层
├── 📁 ViewModels      # 视图模型，处理 UI 逻辑和数据绑定
├── 📁 Views           # UI 视图文件 (.xaml)
├── App.xaml           # 应用程序入口和全局资源
└── TaskManagerApp.sln # 解决方案文件
```

## ✨ 功能特性
任务管理: 支持添加新任务、编辑现有任务详情及删除任务。
数据持久化: 使用本地数据库存储任务数据，确保数据在应用重启后依然存在。
MVVM 设计: 实现了数据绑定 (Data Binding) 和命令 (Commands)，UI 与逻辑分离。
仓储模式: Repositories 层提供了对数据的抽象访问，方便测试和维护。
🛠️ 快速开始
先决条件
Visual Studio 2022 或更高版本
.NET SDK (具体版本请参考 .csproj 文件，通常为 .NET 6.0/7.0/8.0)
SQL Server Express 或 LocalDB (取决于具体数据库配置)
安装与运行
克隆仓库

```bash
git clone https://github.com/mugiEinstein/TaskManagerApp.git
cd TaskManagerApp
```
打开项目 使用 Visual Studio 打开 TaskManagerApp.sln 解决方案文件。

恢复依赖 在解决方案资源管理器中右键点击解决方案，选择“还原 NuGet 包”。

应用数据库迁移 打开“包管理器控制台” (Package Manager Console)，并运行以下命令以创建/更新数据库：

```PowerShell
Update-Database
```
运行应用 按 F5 或点击 Visual Studio 中的“启动”按钮运行应用程序。

🤝 贡献
欢迎提交 Issue 或 Pull Request！如果您想改进此项目：

Fork 本仓库
创建您的特性分支 (git checkout -b feature/AmazingFeature)
提交您的更改 (git commit -m 'Add some AmazingFeature')
推送到分支 (git push origin feature/AmazingFeature)
打开一个 Pull Request
