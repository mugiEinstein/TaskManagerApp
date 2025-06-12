using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Repositories
{
    /// <summary>
    /// 任务数据访问实现，使用 EF Core DbContext
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _context.TaskItems.Include(t => t.Category).ToListAsync();
        }

        public async Task<TaskItem> GetByIdAsync(int id)
        {
            return await _context.TaskItems.Include(t => t.Category)
                                           .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TaskItem item)
        {
            _context.TaskItems.Add(item);
            await _context.SaveChangesAsync();
        }

        //public async Task UpdateAsync(TaskItem item)
        //{
        //    item.UpdatedAt = DateTime.Now;
        //    _context.TaskItems.Update(item);
        //    await _context.SaveChangesAsync();
        //}

        public async Task UpdateAsync(TaskItem item)
        {
            // 先从 DbContext 查询已有实体
            var existing = await _context.TaskItems
                                         .FirstOrDefaultAsync(t => t.Id == item.Id);
            if (existing == null)
                throw new InvalidOperationException("要更新的任务不存在");

            // 将用户修改的属性复制到 existing
            existing.Title = item.Title;
            existing.Description = item.Description;
            existing.DueDate = item.DueDate;
            existing.Priority = item.Priority;
            existing.Status = item.Status;
            existing.CategoryId = item.CategoryId;
            existing.UpdatedAt = DateTime.Now;

            // 如果 Category 导航属性也需要更新可设置 existing.Category = item.Category 等，
            // 但通常只需设置 CategoryId，EF 会处理导航关系。

            // EF Core 会跟踪 existing 实例并检测到属性变更，直接 SaveChanges
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem item)
        {
            _context.TaskItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskItem>> QueryAsync(Expression<Func<TaskItem, bool>> predicate)
        {
            return await _context.TaskItems.Include(t => t.Category)
                                           .Where(predicate).ToListAsync();
        }
    }
}
