// 文件路径：Infrastructure/AppConfig.cs

using System.Text.Json.Serialization;

namespace TaskManagerApp.Infrastructure
{
    /// <summary>
    /// 应用配置模型，存储到 JSON 文件中。
    /// 可以根据需求添加更多配置属性。
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 提前提醒的分钟数（若实现提醒功能），如30表示任务到期前30分钟提醒。
        /// </summary>
        public int ReminderAheadMinutes { get; set; } = 30;

        /// <summary>
        /// SQLite 数据库文件的相对或绝对路径。
        /// 如果为空或默认，则使用 AppDbContext.OnConfiguring 中默认路径(LocalApplicationData 下)。
        /// </summary>
        public string DatabaseFilePath { get; set; } = string.Empty;

        /// <summary>
        /// 界面主题，可自定义枚举或字符串，如 "Light" / "Dark" / "System" 等。
        /// 如果使用 ModernWpf，可在 MainWindow 或 App 启动时根据此值设置 ThemeManager.Current.ApplicationTheme。
        /// </summary>
        public string Theme { get; set; } = "System";

        /// <summary>
        /// 是否启用日志记录。若 false，可在 Logger 初始化或调用时判断是否实际写入。
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        /// 其他自定义配置项，可在此扩展。
        /// </summary>
        // public bool SomeOtherOption { get; set; } = false;
    }
}
