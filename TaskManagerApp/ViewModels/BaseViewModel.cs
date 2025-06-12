using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskManagerApp.ViewModels
{
    /// <summary>
    /// 基础 ViewModel，包含公共属性
    /// </summary>
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
    }
}
