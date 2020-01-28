using Idoneus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {

        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Dashboard;
        public string PageHeader { get; set; }

        public object Parameters { get; private set; }


        public void GoTo(ApplicationPage page, object param = null)
        {
            Parameters = param;
            CurrentPage = page;
           
        }
 
    }
}
