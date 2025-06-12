using System.Windows;

namespace TaskManagerApp.Views
{
    /// <summary>
    /// CategoryManagementWindow code-behind，DataContext 由调用方设置
    /// </summary>

    public partial class CategoryManagementWindow : Window
    {
        public CategoryManagementWindow()
        {
            InitializeComponent();
        }
    }
}

// 修改1
//using System.Windows;
//using System.Windows.Controls;
//using TaskManagerApp.ViewModels;
//using TaskManagerApp.Models;

//namespace TaskManagerApp.Views
//{
//    /// <summary>
//    /// CategoryManagementWindow code-behind
//    /// </summary>
//    public partial class CategoryManagementWindow : Window
//    {
//        private CategoryManagementViewModel ViewModel => DataContext as CategoryManagementViewModel;

//        public CategoryManagementWindow()
//        {
//            InitializeComponent();
//        }

//        private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
//        {
//            if (e.EditAction == DataGridEditAction.Commit)
//            {
//                // 编辑提交后，延后到 UI 更新绑定后执行，以确保绑定值已更新到 Category.Name
//                // 使用 Dispatcher 或延后执行:
//                await Dispatcher.InvokeAsync(async () =>
//                {
//                    if (e.Row.Item is Category category)
//                    {
//                        await ViewModel.UpdateCategoryAsync(category);
//                    }
//                });
//            }
//        }
//    }
//}