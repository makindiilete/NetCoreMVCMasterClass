using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Asp_Net_Core_Masterclass.Models
{
    //This is the class responsible for data representation.. It contains all the properties we want an employee to have
    public class Employee
    {
        public int Id { get; set; }
        //this holds our encryptedId and the "NotMapped" attribute tells Ef core not to store it in the database
        [NotMapped]
        public string EncryptedId { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 Characters")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email Format")]
        //here we specify "Office Email" as the text for our Email Label
        [Display(Name = "Office Email")]
        [Remote(action:"IsEmailInUse", controller:"Account")]
        public string Email { get; set; }
        //Here to add required validation to the Department field, we first need to make it optional by adding "?" and then add d [Required] atribute
        [Required]
        public DeptEnum? Department { get; set; }

        public string PhotoPath { get; set; }
    }
}
