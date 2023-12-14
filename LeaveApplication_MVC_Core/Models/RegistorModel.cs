using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LeaveApplication_MVC_Core.Models
{
    public class RegistorModel
    {
        [DisplayName("Enter your Name")]
        public string name { get; set; }

        [Required, MinLength(3), EmailAddress]
        public string username { get; set; }

        [Required]
        public string role { get; set; }
    }
}
