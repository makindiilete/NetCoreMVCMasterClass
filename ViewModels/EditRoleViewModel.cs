using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class EditRoleViewModel
    {
        //ds constructor is created to initialize our "Users" property to empty list.. ds is required so we can call the "Model.Users.Any()" on it
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
