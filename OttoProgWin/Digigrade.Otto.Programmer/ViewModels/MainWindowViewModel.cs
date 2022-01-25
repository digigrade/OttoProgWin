using Digigrade.Otto.Programmer.MVVM;
using System.Windows.Controls;
using System.Windows.Input;

namespace Digigrade.Otto.Programmer.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private UserControl? _subWindowContent;

        public MainWindowViewModel() : base(null)
        {
        }

        public ICommand CloseCommand => new ViewModelCommand(Close, CanClose);

        public ICommand OpenSomeSubWindowCommand => new ViewModelCommand(OpenSomeSubWindow, CanOpenSomeSubWindow);

        /// <summary>
        /// The sub-window view as a user-control.
        /// </summary>
        public UserControl? SubWindowContent
        {
            get
            {
                return _subWindowContent;
            }
            set
            {
                if (_subWindowContent != value)
                {
                    _subWindowContent = value;
                    RaisePropertyChangedEvent();
                }
            }
        }

        public string Title { get; } = "Otto Desktop App";

        internal bool CanClose() => true;

        internal bool CanOpenSomeSubWindow() => true;

        internal void Close() => RaisePropertyChangedEvent();

        internal void OpenSomeSubWindow()
        {
            OpenSubWindow("Hello World");
        }

        internal void OpenSubWindow(string someModel)
        {
            if (someModel != null)
            {
                //var title = "Edit - " + nameof(someModel);
                //var viewModel = new TesterConfigurationSubWindowViewModel(title, someModel);
                //viewModel.PropertyChanged += SubWindowViewModel_PropertyChanged;
                //SubWindowContent = new TesterConfigurationSubWindowView(viewModel);
            }
        }

        private void SomeSubWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Close) && sender != null && sender is SubWindowViewModel)
            {
                //(sender as SubWindowViewModel).PropertyChanged -= SubWindowViewModel_PropertyChanged;
                //RaisePropertyChangedEvent(nameof(TesterConfigurationNames));
            }
        }
    }
}