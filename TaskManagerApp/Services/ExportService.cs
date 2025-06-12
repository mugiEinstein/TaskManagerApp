using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerApp.Models;

namespace TaskManagerApp.Services
{
    /// <summary>
    /// 导出服务，将任务列表导出到 CSV/TXT
    /// </summary>
    public class ExportService
    {
        public async Task ExportToCsvAsync(List<TaskItem> tasks, string filePath)
        {
            // 确保目录存在
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            // 写表头
            await writer.WriteLineAsync("Id,Title,DueDate,Priority,Status,Category,CreatedAt,UpdatedAt");
            foreach (var t in tasks)
            {
                string due = t.DueDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                string categoryName = t.Category?.Name?.Replace("\"", "\"\"") ?? "";
                string titleEscaped = t.Title?.Replace("\"", "\"\"") ?? "";
                if (titleEscaped.Contains(","))
                    titleEscaped = $"\"{titleEscaped}\"";
                if (categoryName.Contains(","))
                    categoryName = $"\"{categoryName}\"";
                string line = $"{t.Id},{titleEscaped},{due},{t.Priority},{t.Status},{categoryName},{t.CreatedAt:yyyy-MM-dd HH:mm:ss},{t.UpdatedAt:yyyy-MM-dd HH:mm:ss}";
                await writer.WriteLineAsync(line);
            }
        }
    }
}
