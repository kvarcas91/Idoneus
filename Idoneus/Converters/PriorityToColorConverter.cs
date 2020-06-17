using Common;
using Idoneus.Converters.Base;
using System;
using System.Globalization;

namespace Idoneus.Converters
{
    public class PriorityToColorConverter : BaseValueConverter<PriorityToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = "White";

            switch ((Priority)value)
            {
                case Priority.Default:
                    color = "#2196F3";
                    break;
                case Priority.Low:
                    color = "#FFA800";
                    break;
                case Priority.Medium:
                    color = "#FF6A00";
                    break;
                case Priority.High:
                    color = "red";
                    break;
                default:
                    break;
            }

            return color;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
