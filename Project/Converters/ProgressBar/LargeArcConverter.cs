using System;
using System.Globalization;
using System.Windows.Data;

namespace Project.Converters
{
    public class LargeArcConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = (double)values[0];
            var minimum = (double)values[1];
            var maximum = (double)values[2];

            //if ((new[] { value, minimum, maximum }))
              //  return Binding.DoNothing;

            if (values.Length == 4)
            {
                var fullIndeterminateScaling = (double)values[3];
                if (!double.IsNaN(fullIndeterminateScaling) && fullIndeterminateScaling > 0.0)
                {
                    value = (maximum - minimum) * fullIndeterminateScaling;
                }
            }

            var percent = maximum <= minimum ? 1.0 : (value - minimum) / (maximum - minimum);

            return percent > 0.5;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
