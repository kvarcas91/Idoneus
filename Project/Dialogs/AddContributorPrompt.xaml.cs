using Core.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project.Dialogs
{
    /// <summary>
    /// Interaction logic for PromptTest.xaml
    /// </summary>
    public partial class PromptTest : Window
    {

        private static PromptTest instance;

        public PromptTest()
        {
            InitializeComponent();
            instance = this;
        }

        public static int ShowDialog(IList<IContributor> contributors)
        {
            //Window prompt = new Window();
            Window prompt = new PromptTest();
            //prompt.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //prompt.Width = 500;
            //prompt.Height = 100;

            //StackPanel panel = new StackPanel() { Orientation = Orientation.Vertical };

            //Button confirmation = new Button() { Content = "Ok" };
            //confirmation.Click += (sender, e) => { prompt.Close(); };

            //panel.Children.Add(confirmation);
            //prompt.Content = panel;
            //prompt.ShowDialog();
            foreach (var contributor in contributors)
            {
                Console.WriteLine(contributor.ToString());
            }
            prompt.ShowDialog();
            return 46;
        }
    }
}
