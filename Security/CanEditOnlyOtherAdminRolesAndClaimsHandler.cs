using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Asp_Net_Core_Masterclass.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement) // here we specified our requirement class as the type of the 2nd parameter
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInAdminId =
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            //For the user to be able to work with the action method on which this custom authorization policy will be placed, he/she must be in the Admin Role
            if (context.User.IsInRole("Admin") &&
                //he/she must have Edit Role claim and value set to true
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                //the Id of the User he/she want to edit must not be the Id he/she login with i.e. He cannot edit his own Role or claim (depending on where u paste the authorization)
                adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                //if the conditions above is satisfied, then the user succeed to edit the given role
                context.Succeed(requirement);
            }
            //We can return failure if this handler fails i.e. If the user is either not an Admin, He does not have an Edit Role or He/she fails at the two...But by default we do not want to do this, if this handler fails, the next handler will be called....
            //context.Fail();

            return Task.CompletedTask;
        }
    }
}
