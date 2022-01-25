using Digigrade.Otto.Programmer.MVVM;
using System.Windows;
using System.Windows.Input;

namespace Digigrade.Otto.Programmer.ViewModels
{
    public abstract class SubWindowViewModel : ViewModel
    {
        protected bool _isDialog = true;
        protected ResizeMode _resizeMode = ResizeMode.NoResize;
        protected SizeToContent _sizeToContent = SizeToContent.WidthAndHeight;
        protected double _startingHeight = -1;
        protected double _startingWidth = -1;

        public SubWindowViewModel(string title, object model) : base(model)
        {
            Title = title;
        }

        public ICommand CloseCommand => new ViewModelCommand(Close, CanClose);

        /// <summary>
        /// Returns value indicating if the window should behave as a dialog box.
        /// </summary>
        public bool IsDialog { get => _isDialog; }

        public ResizeMode ResizeMode { get => _resizeMode; }

        public SizeToContent SizeToContent { get => _sizeToContent; }

        public double StartingHeight { get => _startingHeight; }

        public double StartingWidth { get => _startingWidth; }

        public string Title { get; }

        public virtual void Close()
        {
            RaisePropertyChangedEvent();
        }

        protected bool CanClose()
        {
            return true;
        }
    }
}