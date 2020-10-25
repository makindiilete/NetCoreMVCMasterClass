using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Asp_Net_Core_Masterclass.Models
{
    public class EmployeeCreateViewModel
    {
        public Employee Employee { get; set; }

//IFormFile type represent a file sent via http request so since we want to upload a photo to the server via POST request, we make this property of this type
        public IFormFile Photo { get; set; }
    }
}
