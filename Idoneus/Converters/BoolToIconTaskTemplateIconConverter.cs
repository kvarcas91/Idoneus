using Idoneus.Converters.Base;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Idoneus.Converters
{
    public class BoolToIconTaskTemplateIconConverter : BaseValueConverter<BoolToIconTaskTemplateIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? PackIconKind.StopCircleOutline : PackIconKind.PlayCircleOutline;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
