using Microsoft.AspNetCore.Identity;

namespace Asp_Net_Core_Masterclass.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}
