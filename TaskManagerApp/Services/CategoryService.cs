using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerApp.Models;
using TaskManagerApp.Repositories;

namespace TaskManagerApp.Services
{
    /// <summary>
    /// 分类业务逻辑服务
    /// </summary>
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly ITaskRepository _taskRepo;

        public CategoryService(ICategoryRepository categoryRepo, ITaskRepository taskRepo)
        {
            _categoryRepo = categoryRepo;
            _taskRepo = taskRepo;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAllAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("分类名称不能为空");
            var existed = (await _categoryRepo.GetAllAsync())
                          .FirstOrDefault(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase));
            if (existed != null)
                throw new InvalidOperationException("已存在相同名称的分类");
            await _categoryRepo.AddAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("分类名称不能为空");
            var list = await _categoryRepo.GetAllAsync();
            if (list.Any(c => c.Id != category.Id && c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("已存在相同名称的分类");
            await _categoryRepo.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            // 删除前检查是否有任务关联
            var tasks = await _taskRepo.QueryAsync(t => t.CategoryId == categoryId);
            if (tasks.Any())
                throw new InvalidOperationException("该分类下存在任务，请先修改或删除关联任务");
            var category = await _categoryRepo.GetByIdAsync(categoryId);
            if (category == null)
                throw new InvalidOperationException("分类不存在");
            await _categoryRepo.DeleteAsync(category);
        }
    }
}
