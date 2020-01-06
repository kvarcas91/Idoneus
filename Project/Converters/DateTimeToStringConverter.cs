using Project.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Converters
{
    public class DateTimeToStringConverter : BaseValueConverter<DateTimeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;

            if (parameter != null)
                return date.ToLocalTime().ToString("dd MMM, yyyy");

            if (date.Date == DateTime.UtcNow.Date)
                //return just time
                return date.ToLocalTime().ToString("HH:mm");

            // if it is not today..
            return date.ToLocalTime().ToString("HH:mm, MMM, yyyy");

            return date;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
