using Digigrade.Otto.Programmer.Properties;
using Digigrade.Otto.Programmer.ViewModels;
using Digigrade.Otto.Programmer.Views;
using System;
using System.Windows;

namespace Digigrade.Otto.Programmer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Save new app instance if first run.
            if (Settings.Default.AppInstance == Guid.Empty)
            {
                Settings.Default.AppInstance = Guid.NewGuid();
                Settings.Default.Save();
            }

            // Save video acceleration
            int displayTier = System.Windows.Media.RenderCapability.Tier;
            if (Settings.Default.VideoAcceleration != displayTier)
            {
                Settings.Default.VideoAcceleration = displayTier;
                Settings.Default.Save();
            }

            // Instantiate a new view-model for the application.
            var mainWindowViewModel = new MainWindowViewModel();
            var mainWindowView = new MainWindowView(mainWindowViewModel);
            var mainWindow = new MainWindow(mainWindowView);
            mainWindow.Show();
        }
    }
}
