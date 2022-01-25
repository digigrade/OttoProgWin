using Digigrade.Otto.Programmer.ViewModels;
using Digigrade.Otto.Programmer.Views;
using Digigrade.Otto.Programmer.Windows;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Digigrade.Otto.Programmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SubWindow? _subWindow;
        private MainWindowViewModel? _viewModel;

        public MainWindow(MainWindowView? content)
        {
            Content = content;
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            if (content != null && content.DataContext != null && content.DataContext is MainWindowViewModel viewModel)
            {
                _viewModel = viewModel;
                Title = _viewModel.Title + " " + fileVersionInfo.FileVersion;
                DataContext = _viewModel;
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_subWindow != null)
            {
                _subWindow.Close();
                _subWindow = null;
            }

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.Dispose();
                _viewModel = null;
            }

            base.OnClosed(e);
        }

        private void EnsureWindowIsWithinVisibleArea()
        {
            if (SystemParameters.VirtualScreenLeft > Left || (SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth) < Left || SystemParameters.VirtualScreenTop > Top || (SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight) < Top)
            {
                // Reset Size
                Height = 1;
                Width = 1;

                // Center On Primary Screen
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double windowWidth = Width;
                double windowHeight = Height;
                Left = (screenWidth / 2) - (windowWidth / 2);
                Top = (screenHeight / 2) - (windowHeight / 2);
            }
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EnsureWindowIsWithinVisibleArea();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.Close))
            {
                Application.Current.Shutdown();
            }
            else if (e.PropertyName == nameof(MainWindowViewModel.SubWindowContent))
            {
                if (_viewModel?.SubWindowContent != null)
                {
                    // Cleanup existing sub window
                    if (_subWindow != null)
                    {
                        _subWindow.Close();
                        _subWindow = null;
                    }

                    // Open new sub window
                    _subWindow = new SubWindow(this, _viewModel.SubWindowContent);
                }
                else
                {
                    // Close sub window
                    if (_subWindow != null)
                    {
                        _subWindow.Close();
                        _subWindow = null;
                    }
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Uncomment code below if overridding system frame
            //if (WindowState == WindowState.Maximized)
            //{
            //    MainWindowMaximizeButton.Visibility = Visibility.Visible;
            //    MainWindowRestoreButton.Visibility = Visibility.Collapsed;
            //    BorderThickness = new Thickness(7);
            //}
            //else
            //{
            //    MainWindowMaximizeButton.Visibility = Visibility.Collapsed;
            //    MainWindowRestoreButton.Visibility = Visibility.Visible;
            //    BorderThickness = new Thickness(0);
            //}
        }
    }
}