using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Infrastructure;
using TaskManagerApp.Repositories;
using TaskManagerApp.Services;
using TaskManagerApp.ViewModels;
using TaskManagerApp.Views;

namespace TaskManagerApp
{
    /// <summary>
    /// 应用启动类，使用 Generic Host 配置 DI, DbContext, Services, ViewModels, Views
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        /// <summary>
        /// 静态属性，便于从任意位置通过 (Application.Current as App).Services 获取 IServiceProvider
        /// </summary>
        public static IServiceProvider Services => (Current as App)?._host.Services;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 预初始化 Logger
            Logger.Info("Application Starting");

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 注册 ConfigService（如需）
                    services.AddSingleton<ConfigService>();

                    // 注册 DbContext（使用 OnConfiguring 中默认 SQLite 路径）
                    services.AddDbContext<AppDbContext>();

                    // 注册 Repositories
                    services.AddScoped<ITaskRepository, TaskRepository>();
                    services.AddScoped<ICategoryRepository, CategoryRepository>();

                    // 注册 Services
                    services.AddScoped<TaskService>();
                    services.AddScoped<CategoryService>();
                    services.AddScoped<ExportService>();
                    services.AddScoped<ReminderService>();

                    // 注册 ViewModels
                    services.AddSingleton<MainViewModel>();
                    services.AddTransient<TaskEditViewModel>();
                    services.AddTransient<CategoryManagementViewModel>();

                    // 注册 Views (窗口)
                    services.AddTransient<MainWindow>();
                    services.AddTransient<TaskEditWindow>();
                    services.AddTransient<CategoryManagementWindow>();
                })
                .Build();

            // 自动迁移数据库
            try
            {
                using var scope = _host.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                Logger.Info("Database migrated/initialized.");
            }
            catch (Exception ex)
            {
                Logger.Error("Database migration failed", ex);
                MessageBox.Show($"数据库初始化失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _host.Start();

            // 显示主窗口
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            Logger.Info("Application Exiting");
            Logger.CloseAndFlush();

            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }
}
