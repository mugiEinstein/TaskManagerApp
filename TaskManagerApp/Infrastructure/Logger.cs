using Serilog;
using System;
using System.IO;

namespace TaskManagerApp.Infrastructure
{
    /// <summary>
    /// 静态 Logger 类，封装 Serilog 日志记录
    /// </summary>
    public static class Logger
    {
        static Logger()
        {
            try
            {
                var folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "TaskManagerApp",
                    "logs");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(
                        path: Path.Combine(folder, "log-.txt"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();

                Log.Information("Logger initialized.");
            }
            catch
            {
                // 忽略初始化失败
            }
        }

        public static void Info(string message)
        {
            try { Log.Information(message); }
            catch { }
        }

        public static void Error(string message, Exception ex = null)
        {
            try
            {
                if (ex != null) Log.Error(ex, message);
                else Log.Error(message);
            }
            catch { }
        }

        public static void Debug(string message)
        {
            try { Log.Debug(message); }
            catch { }
        }

        /// <summary>
        /// 关闭并刷新日志，可在应用退出时调用
        /// </summary>
        public static void CloseAndFlush()
        {
            try { Log.CloseAndFlush(); }
            catch { }
        }
    }
}
