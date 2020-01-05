using Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {

        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Dashboard;

        public void GoTo(ApplicationPage page)
        {
            CurrentPage = page;
        }

    }
}
