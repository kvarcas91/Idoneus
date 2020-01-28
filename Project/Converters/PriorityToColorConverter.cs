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
    public class PriorityToColorConverter : BaseValueConverter<PriorityToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = "White";

            switch ((Priority)value)
            {
                case Priority.Default:
                    color = Application.Current.Resources["PriorityDefault"].ToString();
                    break;
                case Priority.Low:
                    color = Application.Current.Resources["PriorityLow"].ToString();
                    break;
                case Priority.Medium:
                    color = Application.Current.Resources["PriorityMedium"].ToString();
                    break;
                case Priority.High:
                    color = Application.Current.Resources["PriorityHigh"].ToString();
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
