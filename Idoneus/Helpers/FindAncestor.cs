using System.Windows;
using System.Windows.Media;

namespace Idoneus.Helpers
{
    public static class FindAncestor
    {

        public static T Find<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

    }
}
