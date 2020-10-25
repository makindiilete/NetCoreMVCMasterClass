using System.ComponentModel.DataAnnotations;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class ForgotPasswordViewModel
    {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
    }
}
