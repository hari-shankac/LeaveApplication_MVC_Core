using System.ComponentModel.DataAnnotations;

namespace LeaveApplication_MVC_Core.Models
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string token { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string confirmPassword { get; set; }
    }
}
