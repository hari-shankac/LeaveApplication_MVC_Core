using System.ComponentModel.DataAnnotations;

namespace LeaveApplication_MVC_Core.Models
{
    public class LoginModel
    {
        [Required, MinLength(3), EmailAddress]
        public string username { get; set; }

        [Required, StringLength(30), MinLength(6), DataType(DataType.Password)]
        public string password { get; set; }
    }
}
