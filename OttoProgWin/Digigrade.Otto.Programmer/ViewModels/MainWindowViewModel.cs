using Digigrade.Otto.Programmer.MVVM;
using Digigrade.Otto.Scripts;
using Digigrade.Otto.Scripts.Debug;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace Digigrade.Otto.Programmer.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private List<DebugResult> _debugResults = new List<DebugResult>();
        private List<OttoScript> _scripts = new List<OttoScript>();
        private UserControl? _subWindowContent;

        public MainWindowViewModel() : base(null)
        {
        }

        public ICommand BuildCommand => new ViewModelCommand(Build, CanBuild);
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

        internal void Build()
        {
            var sourceFiles = OttoSourceBuilder.Build(_scripts, out List<DebugResult> debugResults);
            _debugResults.Clear();
            _debugResults.AddRange(debugResults);

            if (sourceFiles != null)
            {
                foreach (var file in sourceFiles)
                {
                }
            }
        }

        internal bool CanBuild() => OttoSourceBuilder.CanBuild(_scripts);

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