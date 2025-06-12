// 文件路径：Infrastructure/ConfigService.cs

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskManagerApp.Infrastructure
{
    /// <summary>
    /// 配置服务，负责加载和保存 AppConfig 到本地 JSON 文件。
    /// </summary>
    public class ConfigService
    {
        private readonly string _configFolder;
        private readonly string _configFilePath;
        private AppConfig _config;

        /// <summary>
        /// 构造函数：确定配置文件路径并尝试加载。
        /// </summary>
        public ConfigService()
        {
            // 将配置文件放在 LocalApplicationData\TaskManagerApp\config.json
            _configFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TaskManagerApp");
            if (!Directory.Exists(_configFolder))
            {
                Directory.CreateDirectory(_configFolder);
            }
            _configFilePath = Path.Combine(_configFolder, "config.json");

            // 尝试加载配置文件；若不存在或解析失败，使用默认配置
            try
            {
                if (File.Exists(_configFilePath))
                {
                    string json = File.ReadAllText(_configFilePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    _config = JsonSerializer.Deserialize<AppConfig>(json, options) ?? new AppConfig();
                }
                else
                {
                    _config = new AppConfig();
                    // 立即保存初始默认配置
                    SaveConfig();
                }
            }
            catch (Exception ex)
            {
                // 如果加载或解析出错，记录日志并使用默认配置
                Logger.Error("加载配置文件时发生异常，使用默认配置", ex);
                _config = new AppConfig();
                try
                {
                    SaveConfig();
                }
                catch
                {
                    // 忽略保存异常
                }
            }
        }

        /// <summary>
        /// 获取当前配置对象（内存实例）。修改后应调用 SaveConfig() 保存到文件。
        /// </summary>
        public AppConfig GetConfig()
        {
            return _config;
        }

        /// <summary>
        /// 将当前 _config 保存到配置文件（同步方式）。
        /// </summary>
        public void SaveConfig()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                string json = JsonSerializer.Serialize(_config, options);
                File.WriteAllText(_configFilePath, json);
                Logger.Info("配置已保存到 " + _configFilePath);
            }
            catch (Exception ex)
            {
                Logger.Error("保存配置文件时发生异常", ex);
            }
        }

        /// <summary>
        /// 异步保存配置到文件（可在 UI 线程外调用）。
        /// </summary>
        public async Task SaveConfigAsync()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                string json = JsonSerializer.Serialize(_config, options);
                await File.WriteAllTextAsync(_configFilePath, json);
                Logger.Info("配置已异步保存到 " + _configFilePath);
            }
            catch (Exception ex)
            {
                Logger.Error("异步保存配置文件时发生异常", ex);
            }
        }
    }
}
