using System.Windows;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    /// <summary>
    /// MainWindow code-behind，通过 DI 注入 MainViewModel
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            _vm = vm;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _vm.LoadDataAsync();
        }
    }
}
