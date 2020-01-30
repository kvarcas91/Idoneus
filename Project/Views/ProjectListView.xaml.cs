using Core.DataModels;
using Idoneus.Utils;
using Idoneus.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Idoneus.Views
{
    /// <summary>
    /// Interaction logic for ProjectListView.xaml
    /// </summary>
    public partial class ProjectListView : UserControl
    {
        public ProjectListView()
        {
            InitializeComponent();
        }

        Point startPoint;
        int startIndex = -1;

        private void FileListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("componentItem") || sender != e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void FileListView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAncestor.Find<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null) return;
                IData component = (IData)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                if (component == null) return;

                startIndex = FileListView.SelectedIndex;
                DataObject dragData = new DataObject("componentItem", component);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void OuterFileDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine("OuterFileDrop");
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ((ProjectListViewModel)this.DataContext).OnDropOuterFile(files);
        }

        private void FileListView_Drop(object sender, DragEventArgs e)
        {
            /**
             * Checks if item has been droped from within the list or outside
             */
            if (startIndex < 0)
            {
                OuterFileDrop(sender, e);
                return;
            }

            if (e.Data.GetDataPresent("componentItem") && sender == e.Source)
            {
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAncestor.Find<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null)
                {
                    startIndex = -1;
                    e.Effects = DragDropEffects.None;
                    return;
                }

                var component = (IData)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                var source = (IData)FileListView.Items.GetItemAt(startIndex);
                startIndex = -1;
                ((ProjectListViewModel)this.DataContext).OnDrop(source, component);
            }
        }
    }
}
