using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TaskManagerApp.Models;
using TaskManagerApp.Repositories;

namespace TaskManagerApp.Services
{
    /// <summary>
    /// 任务业务逻辑服务
    /// </summary>
    public class TaskService
    {
        private readonly ITaskRepository _taskRepo;
        private readonly ICategoryRepository _categoryRepo;

        public TaskService(ITaskRepository taskRepo, ICategoryRepository categoryRepo)
        {
            _taskRepo = taskRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepo.GetAllAsync();
        }

        public async Task AddTaskAsync(TaskItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentException("任务标题不能为空");
            if (item.CategoryId.HasValue)
            {
                var cat = await _categoryRepo.GetByIdAsync(item.CategoryId.Value);
                if (cat == null)
                    throw new ArgumentException("所选分类不存在");
            }
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
            await _taskRepo.AddAsync(item);
        }

        public async Task UpdateTaskAsync(TaskItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentException("任务标题不能为空");
            if (item.CategoryId.HasValue)
            {
                var cat = await _categoryRepo.GetByIdAsync(item.CategoryId.Value);
                if (cat == null)
                    throw new ArgumentException("所选分类不存在");
            }
            item.UpdatedAt = DateTime.Now; //
            await _taskRepo.UpdateAsync(item);
        }



        public async Task DeleteTaskAsync(int id)
        {
            var item = await _taskRepo.GetByIdAsync(id);
            if (item == null)
                throw new InvalidOperationException("任务不存在");
            await _taskRepo.DeleteAsync(item);
        }

        public async Task<List<TaskItem>> FilterTasksAsync(
            int? categoryId,
            TaskState? status,
            PriorityLevel? priority,
            DateTime? dueDateFrom,
            DateTime? dueDateTo)
        {
            // 动态组合 Expression
            var predicate = (System.Linq.Expressions.Expression<Func<TaskItem, bool>>)(t => true);
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                predicate = predicate.AndAlso(t => t.CategoryId == categoryId.Value);
            }
            if (status.HasValue)
            {
                predicate = predicate.AndAlso(t => t.Status == status.Value);
            }
            if (priority.HasValue)
            {
                predicate = predicate.AndAlso(t => t.Priority == priority.Value);
            }
            if (dueDateFrom.HasValue)
            {
                predicate = predicate.AndAlso(t => t.DueDate >= dueDateFrom.Value);
            }
            if (dueDateTo.HasValue)
            {
                predicate = predicate.AndAlso(t => t.DueDate <= dueDateTo.Value);
            }
            var list = await _taskRepo.QueryAsync(predicate);
            // 按 DueDate 升序排序，无 DueDate 的放后面
            list = list.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList();
            return list;
        }

        //public async Task<List<TaskItem>> FilterTasksAsync(Expression<Func<TaskItem, bool>> predicate)
        //{
        //    // 直接调用 Repository
        //    var list = await _taskRepo.QueryAsync(predicate);
        //    // 如果需要 Include Category，在 Repository.QueryAsync 已 Include(t => t.Category)
        //    return list;
        //}

    }
}
