using Core;
using Core.ViewModels;
using Project.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project.Views
{
    /// <summary>
    /// Interaction logic for MainContentView.xaml
    /// </summary>
    public partial class MainContentView : UserControl

    {

        public UserControl CurrentPage
        {
            get => (UserControl)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public static readonly DependencyProperty CurrentPageProperty =
          DependencyProperty.Register(nameof(CurrentPage), typeof(UserControl), typeof(MainContentView), new UIPropertyMetadata(CurrentPagePropertyChanged));

        private static void CurrentPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Get the frames

            var contentFrame = (d as MainContentView).content;

            // Remove current page from new page frame
            contentFrame.Content = null;

            // Set the new page content
            contentFrame.Content = e.NewValue;
        }

        public MainContentView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))

                content.Content = (UserControl)new ApplicationPageValueConverter().Convert(IoC.Get<ApplicationViewModel>().CurrentPage);
        }
    }
}
