using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using TaskManagerApp.Models;
using TaskManagerApp.Services;

namespace TaskManagerApp.ViewModels
{
    /// <summary>
    /// 分类管理 ViewModel
    /// </summary>
    public partial class CategoryManagementViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;

        [ObservableProperty]
        private ObservableCollection<Category> categories = new ObservableCollection<Category>();

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private string newCategoryName;

        public CategoryManagementViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            _ = LoadCategoriesAsync();
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                IsBusy = true;
                var list = await _categoryService.GetAllCategoriesAsync();
                Categories.Clear();
                foreach (var c in list)
                    Categories.Add(c);
            }
            catch (Exception ex)
            {
                TaskManagerApp.Infrastructure.Logger.Error("LoadCategoriesAsync 异常", ex);
                MessageBox.Show($"加载分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
            {
                MessageBox.Show("请输入分类名称", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var cat = new Category { Name = NewCategoryName.Trim() };
                await _categoryService.AddCategoryAsync(cat);
                NewCategoryName = string.Empty;
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                TaskManagerApp.Infrastructure.Logger.Error("AddCategoryAsync 异常", ex);
                MessageBox.Show($"新增分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task EditCategoryAsync()
        {
            if (SelectedCategory == null) return;
            // 这里使用简单 InputBox，需要引用 Microsoft.VisualBasic
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "修改分类名称：", "编辑分类", SelectedCategory.Name);
            if (string.IsNullOrWhiteSpace(input)) return;
            try
            {
                SelectedCategory.Name = input.Trim();
                await _categoryService.UpdateCategoryAsync(SelectedCategory);
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                TaskManagerApp.Infrastructure.Logger.Error("EditCategoryAsync 异常", ex);
                MessageBox.Show($"编辑分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategory == null) return;
            var result = MessageBox.Show($"确认删除分类 \"{SelectedCategory.Name}\"？",
                                         "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _categoryService.DeleteCategoryAsync(SelectedCategory.Id);
                    await LoadCategoriesAsync();
                }
                catch (Exception ex)
                {
                    TaskManagerApp.Infrastructure.Logger.Error("DeleteCategoryAsync 异常", ex);
                    MessageBox.Show($"删除分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

// 修改1
//namespace TaskManagerApp.ViewModels
//{
//    /// <summary>
//    /// 分类管理 ViewModel，支持 DataGrid 内联编辑
//    /// </summary>
//    public partial class CategoryManagementViewModel : BaseViewModel
//    {
//        private readonly CategoryService _categoryService;

//        [ObservableProperty]
//        private ObservableCollection<Category> categories = new ObservableCollection<Category>();

//        [ObservableProperty]
//        private Category selectedCategory;

//        [ObservableProperty]
//        private string newCategoryName;

//        public CategoryManagementViewModel(CategoryService categoryService)
//        {
//            _categoryService = categoryService;
//            _ = LoadCategoriesAsync();
//        }

//        /// <summary>
//        /// 加载所有分类
//        /// </summary>
//        public async Task LoadCategoriesAsync()
//        {
//            try
//            {
//                IsBusy = true;
//                var list = await _categoryService.GetAllCategoriesAsync();
//                Categories.Clear();
//                foreach (var c in list)
//                    Categories.Add(c);
//            }
//            catch (Exception ex)
//            {
//                TaskManagerApp.Infrastructure.Logger.Error("LoadCategoriesAsync 异常", ex);
//                MessageBox.Show($"加载分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        /// <summary>
//        /// 添加新分类
//        /// </summary>
//        [RelayCommand]
//        private async Task AddCategoryAsync()
//        {
//            if (string.IsNullOrWhiteSpace(NewCategoryName))
//            {
//                MessageBox.Show("请输入分类名称", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return;
//            }
//            try
//            {
//                var cat = new Category { Name = NewCategoryName.Trim() };
//                await _categoryService.AddCategoryAsync(cat);
//                NewCategoryName = string.Empty;
//                await LoadCategoriesAsync();
//            }
//            catch (Exception ex)
//            {
//                TaskManagerApp.Infrastructure.Logger.Error("AddCategoryAsync 异常", ex);
//                MessageBox.Show($"新增分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// 删除分类（用于 DataGrid 中删除命令）
//        /// </summary>
//        [RelayCommand]
//        private async Task DeleteCategoryAsync(Category category)
//        {
//            if (category == null) return;
//            var result = MessageBox.Show($"确认删除分类 \"{category.Name}\"？",
//                                         "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
//            if (result == MessageBoxResult.Yes)
//            {
//                try
//                {
//                    await _categoryService.DeleteCategoryAsync(category.Id);
//                    Categories.Remove(category);
//                }
//                catch (Exception ex)
//                {
//                    TaskManagerApp.Infrastructure.Logger.Error("DeleteCategoryAsync 异常", ex);
//                    MessageBox.Show($"删除分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
//                    // 重新加载以还原
//                    await LoadCategoriesAsync();
//                }
//            }
//        }

//        /// <summary>
//        /// 内联编辑完成时调用，保存更改
//        /// </summary>
//        public async Task UpdateCategoryAsync(Category category)
//        {
//            if (category == null) return;
//            try
//            {
//                if (string.IsNullOrWhiteSpace(category.Name))
//                {
//                    MessageBox.Show("分类名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    // 重新加载以还原旧值
//                    await LoadCategoriesAsync();
//                    return;
//                }
//                await _categoryService.UpdateCategoryAsync(category);
//                // 可显示提示
//                // MessageBox.Show("分类已更新", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//            catch (Exception ex)
//            {
//                TaskManagerApp.Infrastructure.Logger.Error("UpdateCategoryAsync 异常", ex);
//                MessageBox.Show($"更新分类失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
//                // 重新加载列表还原
//                await LoadCategoriesAsync();
//            }
//        }
//    }
//}