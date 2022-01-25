using Digigrade.Otto.Programmer.ViewModels;
using System.Windows.Controls;

namespace Digigrade.Otto.Programmer.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : UserControl
    {
        public MainWindowView(MainWindowViewModel dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}