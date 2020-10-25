using Asp_Net_Core_Masterclass.Models;
using Microsoft.AspNetCore.Http;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class EmployeeEditViewModel : Employee
    {
        //This field contains the path to user existing photo.. We
        public string ExistingPhotoPath { get; set; }
        public IFormFile Photo { get; set; }
    }
}
