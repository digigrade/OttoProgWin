using Digigrade.Otto.Programmer.Properties;
using System.Windows.Data;

namespace Digigrade.Otto.Programmer.Windows
{
    /// <summary>
    /// Provides binding for a window to the application settings.
    /// </summary>
    public class WindowSettingsBinding : Binding
    {
        /// <inheritdoc/>
        public WindowSettingsBinding()
        {
            Initialize();
        }

        /// <inheritdoc/>
        public WindowSettingsBinding(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            Source = Settings.Default;
            Mode = BindingMode.TwoWay;
        }
    }
}
