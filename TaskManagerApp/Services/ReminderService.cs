using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagerApp.Services
{
    /// <summary>
    /// 提醒服务：定时检查即将到期的任务并通过事件通知
    /// </summary>
    public class ReminderService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _aheadTime;
        private readonly HashSet<int> _alreadyReminded = new HashSet<int>();

        /// <summary>
        /// 订阅此事件以在 UI 提示
        /// </summary>
        public event EventHandler<TaskItem> OnReminderDue;

        public ReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // 默认提前时间可从配置读取；这里硬编码 30 分钟，也可改为从 ConfigService.GetConfig().ReminderAheadMinutes
            _aheadTime = TimeSpan.FromMinutes(30);
            Start();
        }

        public void Start()
        {
            _timer = new Timer(async _ => await CheckRemindersAsync(), null, TimeSpan.Zero, _checkInterval);
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async Task CheckRemindersAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var now = DateTime.Now;
                var target = now.Add(_aheadTime);
                var tasks = await db.TaskItems
                    .Include(t => t.Category)
                    .Where(t => t.Status == TaskState.Pending
                                && t.DueDate != null
                                && t.DueDate >= now
                                && t.DueDate <= target)
                    .ToListAsync();
                foreach (var t in tasks)
                {
                    if (!_alreadyReminded.Contains(t.Id))
                    {
                        _alreadyReminded.Add(t.Id);
                        OnReminderDue?.Invoke(this, t);
                    }
                }
                // 清理已过期的提醒记录
                _alreadyReminded.RemoveWhere(id =>
                    db.TaskItems.All(t => t.Id != id || t.DueDate < now));
            }
            catch (Exception ex)
            {
                // 记录日志
                TaskManagerApp.Infrastructure.Logger.Error("ReminderService 异常", ex);
            }
        }
    }
}
