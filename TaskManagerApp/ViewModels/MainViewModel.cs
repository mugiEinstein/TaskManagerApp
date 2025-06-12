using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManagerApp.Models;
using TaskManagerApp.Services;
using TaskManagerApp.Views;
using TaskManagerApp.Infrastructure;
    

namespace TaskManagerApp.ViewModels
{
    /// <summary>
    /// 主界面 ViewModel
    /// </summary>
    public partial class MainViewModel : BaseViewModel
    {
        private readonly TaskService _taskService;
        private readonly CategoryService _categoryService;
        private readonly ExportService _exportService;
        private readonly ReminderService _reminderService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();

        [ObservableProperty]
        private ObservableCollection<Category> categories = new ObservableCollection<Category>();

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private TaskState? selectedStatus;  // 使用 TaskState

        [ObservableProperty]
        private PriorityLevel? selectedPriority;

        [ObservableProperty]
        private DateTime? dueDateFrom;

        [ObservableProperty]
        private DateTime? dueDateTo;

        [ObservableProperty]
        private TaskItem selectedTask;

        public MainViewModel(
            IServiceProvider serviceProvider,
            TaskService taskService,
            CategoryService categoryService,
            ExportService exportService,
            ReminderService reminderService)
        {
            _serviceProvider = serviceProvider;
            _taskService = taskService;
            _categoryService = categoryService;
            _exportService = exportService;
            _reminderService = reminderService;

            Tasks = new ObservableCollection<TaskItem>();
            Categories = new ObservableCollection<Category>();

            // 如果实现提醒，订阅事件
            _reminderService.OnReminderDue += ReminderService_OnReminderDue;
        }

        private void ReminderService_OnReminderDue(object sender, TaskItem e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"任务 \"{e.Title}\" 即将到期：{e.DueDate:yyyy-MM-dd HH:mm}",
                                "任务提醒", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                var cats = await _categoryService.GetAllCategoriesAsync();
                Categories.Clear();
                // 插入 “全部” 选项
                Categories.Add(new Category { Id = 0, Name = "全部" });
                foreach (var c in cats)
                    Categories.Add(c);
                SelectedCategory = Categories.FirstOrDefault();

                await FilterTasksAsync();
            }
            catch (Exception ex)
            {
                TaskManagerApp.Infrastructure.Logger.Error("LoadDataAsync 异常", ex);
                MessageBox.Show($"加载数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// 状态列表绑定，为 ComboBox 提供 enum? 列表
        /// </summary>
        public IEnumerable<TaskState?> StatusList => new TaskState?[] { null, TaskState.Pending, TaskState.Completed };

        /// <summary>
        /// 优先级列表
        /// </summary>
        public IEnumerable<PriorityLevel?> PriorityList => new PriorityLevel?[] { null, PriorityLevel.Low, PriorityLevel.Medium, PriorityLevel.High };

        [RelayCommand]
        private async Task FilterTasksAsync()
        {
            try
            {
                IsBusy = true;
                int? catId = SelectedCategory != null && SelectedCategory.Id != 0 ? SelectedCategory.Id : (int?)null;
                var list = await _taskService.FilterTasksAsync(
                    catId,
                    SelectedStatus,
                    SelectedPriority,
                    DueDateFrom,
                    DueDateTo);
                Tasks.Clear();
                foreach (var t in list)
                    Tasks.Add(t);
            }
            catch (Exception ex)
            {
                TaskManagerApp.Infrastructure.Logger.Error("FilterTasksAsync 异常", ex);
                MessageBox.Show($"筛选失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddTaskAsync()
        {
            var vm = _serviceProvider.GetRequiredService<TaskEditViewModel>();
            vm.Initialize(null);
            var window = _serviceProvider.GetRequiredService<TaskEditWindow>();
            window.DataContext = vm;
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                await FilterTasksAsync();
            }
        }

        [RelayCommand]
        private async Task EditTaskAsync()
        {
            if (SelectedTask == null) return;
            var vm = _serviceProvider.GetRequiredService<TaskEditViewModel>();
            vm.Initialize(SelectedTask);
            var window = _serviceProvider.GetRequiredService<TaskEditWindow>();
            window.DataContext = vm;
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                await FilterTasksAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteTaskAsync()
        {
            if (SelectedTask == null) return;
            var result = MessageBox.Show($"确认删除任务 \"{SelectedTask.Title}\"？",
                                         "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _taskService.DeleteTaskAsync(SelectedTask.Id);
                    Tasks.Remove(SelectedTask);
                }
                catch (Exception ex)
                {
                    TaskManagerApp.Infrastructure.Logger.Error("DeleteTaskAsync 异常", ex);
                    MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task ExportTasksAsync()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv|文本文件 (*.txt)|*.txt",
                FileName = "TasksExport"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    await _exportService.ExportToCsvAsync(Tasks.ToList(), dlg.FileName);
                    MessageBox.Show("导出成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    TaskManagerApp.Infrastructure.Logger.Error("ExportTasksAsync 异常", ex);
                    MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private void ManageCategories()
        {
            var vm = _serviceProvider.GetRequiredService<CategoryManagementViewModel>();
            var window = _serviceProvider.GetRequiredService<CategoryManagementWindow>();
            window.DataContext = vm;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
            // 关闭后刷新分类与任务列表
            _ = LoadDataAsync();
        }
    }
}
