using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TaskManagerApp.Models;

namespace TaskManagerApp.Repositories
{
    /// <summary>
    /// 任务数据访问接口
    /// </summary>
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<TaskItem> GetByIdAsync(int id);
        Task AddAsync(TaskItem item);
        Task UpdateAsync(TaskItem item);
        Task DeleteAsync(TaskItem item);
        Task<List<TaskItem>> QueryAsync(Expression<Func<TaskItem, bool>> predicate);
    }
}