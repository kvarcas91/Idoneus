using Idoneus.Converters.Base;
using System;
using System.Globalization;

namespace Idoneus.Converters
{
    public class ProgressConverter : BaseValueConverter<ProgressConverter>
    {

        // param - ignore difference between today and older days
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var progress = (double)value;

            // Ignore difference between days if true

            if (progress == 100) return 99.9D;
            else return progress;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
