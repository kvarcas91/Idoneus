using Common;
using Idoneus.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace Idoneus.Converters
{
    class StatusToTextDecorationConverter : BaseValueConverter<StatusToTextDecorationConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Status)value == Status.Completed ? TextDecorations.Strikethrough : null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
