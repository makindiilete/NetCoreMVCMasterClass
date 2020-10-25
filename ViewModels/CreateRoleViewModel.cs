using System.ComponentModel.DataAnnotations;

namespace Asp_Net_Core_Masterclass.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
       [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
