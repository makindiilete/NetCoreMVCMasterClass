using System.Collections.Generic;
using System.Security.Claims;

namespace Asp_Net_Core_Masterclass.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>
        {
            //Our first claim
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role")
        };
    }
}
