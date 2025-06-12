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
    /// 任务编辑 ViewModel，用于新增和编辑
    /// </summary>
    public partial class TaskEditViewModel : BaseViewModel
    {
        private readonly TaskService _taskService;
        private readonly CategoryService _categoryService;

        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private DateTime? dueDate;

        [ObservableProperty]
        private PriorityLevel priority;

        [ObservableProperty]
        private TaskState status;

        [ObservableProperty]
        private Category selectedCategory;

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        public TaskEditViewModel(TaskService taskService, CategoryService categoryService)
        {
            _taskService = taskService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// 初始化：task 为 null 表示新增，否则为编辑
        /// </summary>
        public async void Initialize(TaskItem task)
        {
            // 加载分类列表
            Categories.Clear();
            var cats = await _categoryService.GetAllCategoriesAsync();
            foreach (var c in cats)
                Categories.Add(c);

            if (task == null)
            {
                Id = 0;
                Title = string.Empty;
                Description = string.Empty;
                DueDate = null;
                Priority = PriorityLevel.Medium;
                Status = TaskState.Pending;
                SelectedCategory = null;
            }
            else
            {
                Id = task.Id;
                Title = task.Title;
                Description = task.Description;
                DueDate = task.DueDate;
                Priority = task.Priority;
                Status = task.Status;
                SelectedCategory = task.Category;
            }
        }

        public IEnumerable<PriorityLevel> PriorityOptions => Enum.GetValues(typeof(PriorityLevel)) as PriorityLevel[];
        public IEnumerable<TaskState> StatusOptions => Enum.GetValues(typeof(TaskState)) as TaskState[];

        [RelayCommand]
        private async Task SaveAsync(Window window)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    MessageBox.Show("任务标题不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var item = new TaskItem
                {
                    Id = Id,
                    Title = Title,
                    Description = Description,
                    DueDate = DueDate,
                    Priority = Priority,
                    Status = Status,
                    CategoryId = SelectedCategory?.Id
                };
                if (Id == 0)
                {
                    await _taskService.AddTaskAsync(item);
                }
                else
                {
                    await _taskService.UpdateTaskAsync(item);
                }
                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                TaskManagerApp.Infrastructure.Logger.Error("TaskEditViewModel SaveAsync 异常", ex);
                MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
