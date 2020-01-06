
using Project.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Converters
{

    /// <summary>
    /// Returns visibility based on List.Count
    /// paramter = DoInverse
    /// </summary>
    public class ListItemCountToVisibilityConverter : BaseValueConverter<ListItemCountToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            
            if (parameter == null)
                return (int)value > 0 ? Visibility.Collapsed : Visibility.Visible;
            else
                return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            
            throw new NotImplementedException();
        }
    }
}
