﻿using Common;
using Idoneus.Converters.Base;
using System;
using System.Globalization;

namespace Idoneus.Converters
{
    public class StatusToColorConverter : BaseValueConverter<StatusToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = "White";
            var status = (Status)value;
            ViewType viewType = (ViewType)((int)status);

            switch (viewType)
            {
                case ViewType.Archived:
                    color = "gray";
                    break;
                case ViewType.InProgress:
                    color = "#2196F3";
                    break;
                case ViewType.Missed:
                    color = "red";
                    break;
                case ViewType.Completed:
                    color = "green";
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
