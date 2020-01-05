using Project.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Project.Converters
{
    /// <summary>
    /// If value is true, returns strikethrough decoration for a textblock
    /// </summary>
    public class BoolToTextDecorationConverter : BaseValueConverter<BoolToTextDecorationConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (bool)value ? TextDecorations.Strikethrough : Binding.DoNothing;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
