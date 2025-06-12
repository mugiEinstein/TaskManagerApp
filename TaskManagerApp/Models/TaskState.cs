using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApp.Models
{
    /// <summary>
    /// 任务状态枚举：Pending 表示未完成，Completed 表示已完成
    /// 重命名自 TaskStatus，以避免与 System.Threading.Tasks.TaskStatus 冲突。
    /// </summary>
    public enum TaskState
    {
        Pending,
        Completed
    }
}