using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Asp_Net_Core_Masterclass.Security
{
    public class SuperAdminHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            //we check if the user is in Super Admin role
            if (context.User.IsInRole("Super Admin"))
            {
                //if the conditions above is satisfied, then the user succeed to edit the given role
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
