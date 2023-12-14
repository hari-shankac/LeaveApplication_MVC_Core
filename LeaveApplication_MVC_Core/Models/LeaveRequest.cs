using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveApplication_MVC_Core.Models
{
    public class LeaveRequest
    {
        public int leaveId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; } 
        public string leaveRession { get; set; }

        public int hrSatus { get; set; } 
        public int gmSatus { get; set; } 
        public string gmRession { get; set; }

        public int amSatus { get; set; } 
        public string amRession { get; set; }

        public int mSatus { get; set; } 
        public string mRession { get; set; }

        public int tlSatus { get; set; } 
        public string tlRession { get; set; }

        public int priority { get; set; }

        public string Id { get; set; }

    }
}

