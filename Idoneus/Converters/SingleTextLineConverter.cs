using Idoneus.Converters.Base;
using System;
using System.Globalization;

namespace Idoneus.Converters
{
    public class SingleTextLineConverter : BaseValueConverter<SingleTextLineConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            s = s.Replace(Environment.NewLine, " ");
            return s;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
