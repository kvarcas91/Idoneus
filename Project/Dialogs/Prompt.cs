using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project.Dialogs
{
    public static class Prompt
    {
        public static int ShowDialog(string text, string caption)
        {
            Window prompt = new Window();
            prompt.Width = 500;
            prompt.Height = 100;
            prompt.Title = caption;
            TextBlock textLabel = new TextBlock() {Text = text };

            StackPanel panel = new StackPanel() { Orientation = Orientation.Vertical };
           
            Button confirmation = new Button() { Content = "Ok"};
            confirmation.Click += (sender, e) => { prompt.Close(); };
            panel.Children.Add(textLabel);
            prompt.Content = panel;
            prompt.ShowDialog();
            return 46;
        }
    }
}
