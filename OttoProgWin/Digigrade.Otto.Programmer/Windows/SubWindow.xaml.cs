using Digigrade.Otto.Programmer.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Digigrade.Otto.Programmer.Windows
{
    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        /// <summary>
        /// Sub window that can be used as a dialog box or additional window.
        /// <para>NOTE: A dialog box will disable all other windows until closed.</para>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="content"></param>
        /// <param name="isDialog"></param>
        public SubWindow(Window owner, UserControl content)
        {
            Owner = owner;
            Content = content;
            bool isDialog = true;

            if ((Content as UserControl)?.DataContext is SubWindowViewModel viewModel)
            {
                isDialog = viewModel.IsDialog;
                Title = viewModel.Title;
                ResizeMode = viewModel.ResizeMode;
                SizeToContent = viewModel.SizeToContent;
                if (viewModel?.StartingWidth > 0) { Width = viewModel.StartingWidth; }
                if (viewModel?.StartingHeight > 0) { Height = viewModel.StartingHeight; }
                if (viewModel != null)
                {
                    viewModel.PropertyChanged += SubWindowContentViewModel_PropertyChanged;
                }
            }

            InitializeComponent();

            if (isDialog)
            {
                this.ShowDialog();
            }
            else
            {
                this.Show();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if ((Content as UserControl)?.DataContext is SubWindowViewModel viewModel)
            {
                viewModel.PropertyChanged -= SubWindowContentViewModel_PropertyChanged;
            }

            if (Owner is not null)
            {
                Owner.IsEnabled = true;
                Owner = null;
            }

            base.OnClosed(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (Owner is not null)
            {
                Owner.IsEnabled = false;
            }

            base.OnInitialized(e);
        }

        private void SubWindowContentViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SubWindowViewModel.Close))
            {
                Close();
            }
        }
    }
}