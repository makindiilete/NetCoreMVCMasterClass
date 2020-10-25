namespace Asp_Net_Core_Masterclass.ViewModels
{
    //We are using this view model to manage users role when they click on "Manage Role" button on the EditUser page
    public class UserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
