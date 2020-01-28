using Core.Utils;
using Idoneus.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Idoneus.Converters
{
    public class IsCompletedToColorConverter : BaseValueConverter<IsCompletedToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            return (bool)value ? "Gray" : Application.Current.Resources["SecondaryColorBrush"].ToString(); 
            
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
