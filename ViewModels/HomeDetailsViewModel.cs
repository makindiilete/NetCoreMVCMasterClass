using Asp_Net_Core_Masterclass.Models;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class HomeDetailsViewModel
    {
        //this field represent the employee details (Id, Name, Email, & Dept) and dts why its type is "Employee"
        public Employee Employee { get; set; }

        //This field represent the pageTitle we want to render on the Employee details page
        public string PageTitle { get; set; }
    }
}
