using Common;
using Idoneus.Converters.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Idoneus.Converters
{
    public class StatusToBoolConverter : BaseValueConverter<StatusToBoolConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (Status)value;
            return status == Status.Completed ? true : false;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (bool)value;
            return status ? Status.Completed : Status.InProgress;
        }
    }
}
