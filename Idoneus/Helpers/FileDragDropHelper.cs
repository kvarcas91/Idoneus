using Domain.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Idoneus.Helpers
{
    public class FileDragDropHelper
    {

        private static int startIndex = -1;

        public static bool GetIsFileDragDropEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFileDragDropEnabledProperty);
        }

        public static void SetIsFileDragDropEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFileDragDropEnabledProperty, value);
        }

        public static bool GetFileDragDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(FileDragDropTargetProperty);
        }

        public static void SetFileDragDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(FileDragDropTargetProperty, value);
        }

        public static object GetMouseMove(DependencyObject obj)
        {
            return (object)obj.GetValue(MouseMoveProperty);
        }

        public static void SetMouseMove(DependencyObject obj, object value)
        {
            obj.SetValue(MouseMoveProperty, value);
        }

        public static readonly DependencyProperty IsFileDragDropEnabledProperty =
            DependencyProperty.RegisterAttached("IsFileDragDropEnabled", typeof(bool), typeof(FileDragDropHelper), new PropertyMetadata(OnFileDragDropEnabled));

        public static readonly DependencyProperty FileDragDropTargetProperty =
                DependencyProperty.RegisterAttached("FileDragDropTarget", typeof(object), typeof(FileDragDropHelper), null);

        public static readonly DependencyProperty MouseMoveProperty =
          DependencyProperty.RegisterAttached("MouseMove", typeof(object), typeof(FileDragDropHelper), new PropertyMetadata(OnMouseMoveEnabled));

        private static void OnFileDragDropEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            if (d is Control control) control.Drop += OnDrop;
        }

        private static void OnMouseMoveEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            if (d is Control control) control.MouseMove += OnMouseMove;
        }

        private static void OnDrop(object _sender, DragEventArgs _dragEventArgs)
        {

            if (startIndex > -1)
            {
                InnerFileDrop(_sender, _dragEventArgs);
                return;
            }

            if (!(_sender is DependencyObject d)) return;
            var target = d.GetValue(FileDragDropTargetProperty);
            if (target is IFileDragDropTarget fileTarget)
            {
                if (_dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    fileTarget.OnFileDrop((string[])_dragEventArgs.Data.GetData(DataFormats.FileDrop));
                }
            }
            else
            {
                throw new Exception("FileDragDropTarget object must be of type IFileDragDropTarget");
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point startPoint;
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

                startIndex = listView.SelectedIndex;

                DataObject dragData = new DataObject("componentItem", component);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
               
            }
        }

        private static void InnerFileDrop(object sender, DragEventArgs _dragEventArgs)
        {
           

            if (_dragEventArgs.Data.GetDataPresent("componentItem") && sender == _dragEventArgs.Source)
            {
              
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAncestor.Find<ListViewItem>((DependencyObject)_dragEventArgs.OriginalSource);
                if (listViewItem == null)
                {
                    _dragEventArgs.Effects = DragDropEffects.None;
                    startIndex = -1;
                    return;
                }

                var destinationComponent = (IData)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                if (destinationComponent is ProjectFile)
                {
                    startIndex = -1;
                    return;
                }
               
                var source = (IData)listView.Items.GetItemAt(startIndex);

                startIndex = -1;
                if (destinationComponent.Equals(source)) return;

                var d = sender as DependencyObject;
                var target = d.GetValue(FileDragDropTargetProperty);
                if (target is IFileDragDropTarget fileTarget)
                {
                      fileTarget.OnInnerFileDrop(source, destinationComponent); 
                }
                else
                {
                    throw new Exception("FileDragDropTarget object must be of type IFileDragDropTarget");
                }
               
            }
            else
            {
                startIndex = -1;
            }
        }
    }
}
