using System.ComponentModel.DataAnnotations;

namespace LeaveApplication_MVC_Core.Models
{
    public class Employee
    {
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        public string Role { get; set; }
    }
}
