using Core.ViewModels.Base;
using Project.Converters.Base;
using Project.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Converters
{
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            // Find the appropriate page
            switch ((ApplicationPage)value)
            {

                case ApplicationPage.Dashboard:
                    return new DashboardView();
                case ApplicationPage.Projects:
                    return new ProjectListView();
                case ApplicationPage.Tasks:
                    return new TasksView();
                default:
                    Debugger.Break();
                    return null;
            }
           
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
