using System.Collections.Generic;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class UserClaimsViewModel
    {
        //we create a consturctor to initialize a new instance of claims in other to avoid null reference exception.
        public UserClaimsViewModel()
        {
            Claims = new List<UserClaim>();
        }

        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
