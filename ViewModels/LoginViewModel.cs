using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        //This property contain the url the user try to access before he/she was redirected to external authentication provider
        public string ReturnUrl { get; set; }

        // This consist all the external providers configured for our app...
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
    }
