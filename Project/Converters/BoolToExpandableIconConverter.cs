using Project.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace Project.Converters
{
    public class BoolToExpandableIconConverter : BaseValueConverter<BoolToExpandableIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            return (bool)value ? PackIconKind.ExpandLess : PackIconKind.ExpandMore;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
