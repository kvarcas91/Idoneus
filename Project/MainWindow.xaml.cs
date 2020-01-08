using Core.Utils;
using Project.ViewModel;
using System.Windows;

namespace Project
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
