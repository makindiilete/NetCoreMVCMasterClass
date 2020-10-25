using System.ComponentModel.DataAnnotations;
using Asp_Net_Core_Masterclass.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action:"IsEmailInUse", controller:"Account")]
       // [ValidEmailDomain(allowedDomain:"omoakin.com", ErrorMessage = "Email Domain must be omoakin.com")] //ds is our "ValidEmailDomainAttribute.cs", we pass to it the domain name we want to allow for registration
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
        //ds is defined in our ApplicationUser.cs
        public string City { get; set; }
    }
}
