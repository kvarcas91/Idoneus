using Core.Utils;
using Idoneus.ViewModel;
using System.Windows;

namespace Idoneus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            

            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }
    }
}
