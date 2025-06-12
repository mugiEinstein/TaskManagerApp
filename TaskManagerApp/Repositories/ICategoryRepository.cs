using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Models;

namespace TaskManagerApp.Repositories
{
    /// <summary>
    /// 分类数据访问接口
    /// </summary>
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}