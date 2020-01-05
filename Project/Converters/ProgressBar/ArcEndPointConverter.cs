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
    public class ArcEndPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var actualWidth = (double)values[0];
            var value = (double)values[1];
            var minimum = (double)values[2];
            var maximum = (double)values[3];

            //if (new[] { actualWidth, value, minimum, maximum }.AnyNan())
            //    return Binding.DoNothing;

            if (values.Length == 5)
            {
                var fullIndeterminateScaling = (double)values[4];
                if (!double.IsNaN(fullIndeterminateScaling) && fullIndeterminateScaling > 0.0)
                {
                    value = (maximum - minimum) * fullIndeterminateScaling;
                }
            }

            var percent = maximum <= minimum ? 1.0 : (value - minimum) / (maximum - minimum);
            var degrees = 360 * percent;
            if (degrees == 360) degrees = 359.99; 
            var radians = degrees * (Math.PI / 180);

            var centre = new Point(actualWidth / 2, actualWidth / 2);
            var hypotenuseRadius = (actualWidth / 2);

            var adjacent = Math.Cos(radians) * hypotenuseRadius;
            var opposite = Math.Sin(radians) * hypotenuseRadius;

            return new Point(centre.X + opposite, centre.Y - adjacent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
