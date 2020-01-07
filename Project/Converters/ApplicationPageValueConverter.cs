using Core;
using Core.ViewModels;
using Core.ViewModels.Base;
using Project.Converters.Base;
using Project.Views;
using System;
using System.Diagnostics;
using System.Globalization;

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
                    IoC.Get<ApplicationViewModel>().PageHeader = "Dashboard";
                    return new DashboardView();
                case ApplicationPage.Projects:
                    IoC.Get<ApplicationViewModel>().PageHeader = "Projects";
                    return new ProjectListView();
                case ApplicationPage.Tasks:
                    IoC.Get<ApplicationViewModel>().PageHeader = "Tasks";
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
