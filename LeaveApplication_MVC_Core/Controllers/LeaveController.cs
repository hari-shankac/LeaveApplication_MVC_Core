using LeaveApplication_MVC_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeaveApplication_MVC_Core.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {

        private string url = "https://localhost:7183/api/Leave/";
        private HttpClient client = new HttpClient();
        private readonly IConfiguration _configuration;
      

        public LeaveController(IConfiguration configuration)
        {
            _configuration = configuration;
           
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateLeave()
        {

            return View();
        }
        public int YourAction(string role)
        {
            var workflowSection = _configuration.GetSection("Workflow");
            var rolesSection = workflowSection.GetSection("Roles");

            // Accessing individual roles and priorities
            foreach (var roles in rolesSection.GetChildren())
            {
                var roleName = roles.GetValue<string>("Role");
                var priority = roles.GetValue<int>("Priority");
                if(role==roleName)
                {
                    return priority;
                }
                
            }
            return 0;  
        }

        [HttpPost]
        public IActionResult CreateLeave(LeaveRequestModel leave)
        {
          
                var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                var rolePriority = YourAction(role);
                leave.Id = value;
                leave.priority = rolePriority;

                var data = JsonConvert.SerializeObject(leave);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/Json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {   
                    TempData["add"] = "added Sucessfully";
                    return RedirectToAction("Index", "Home");
                }
            
            return View();
        }


        [HttpGet]
       
        public IActionResult showLeave(LeaveRequest leave)
        {

            List<LeaveRequest> products = new List<LeaveRequest>();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var rolePriority = YourAction(role);
        
            HttpResponseMessage response = client.GetAsync(url+"priority?priority="+rolePriority).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<List<LeaveRequest>>(result);
                if (data != null)
                {
                    products = data;
                }
            }
            return View(products);
        }

        [Authorize(Roles ="Hr")]
        public async Task<IActionResult> ShowAllLeaves()
        {
            List<LeaveRequest> leaves = new List<LeaveRequest>();
            

            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<List<LeaveRequest>>(result);
                if (data != null)
                {
                    leaves = data;
                }
            }
            return View(leaves);
        }

        [HttpPost]
        public async Task<IActionResult> Approved(int id, string reason)
        {
            LeaveRequest leaves = new LeaveRequest();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            HttpResponseMessage response = client.GetAsync(url+ "details?id="+id).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<LeaveRequest>(result);
                if (data != null)
                {
                    leaves = data;
                }
            }

            if(role== "Hr")
            {
                if (leaves.gmSatus == (int)RequestEnum.Request.Approval || leaves.gmSatus == (int)RequestEnum.Request.Reject)
                {
                    leaves.hrSatus = (int)RequestEnum.Request.Approval;
                }
                else
                {
                    TempData["error"] = "General Manager not Take any action on it !!!";
                    return BadRequest(TempData["error"]);
                }
            }
            if (role == "Manager")
            {
                if(leaves.tlSatus == (int) RequestEnum.Request.Approval || leaves.tlSatus == (int) RequestEnum.Request.Reject)
                {
                    leaves.mRession = reason;
                    leaves.mSatus = (int)RequestEnum.Request.Approval;
                }
                else
                {
                    TempData["error"] = "Team Leader not Take any action on it !!!";
                    return BadRequest(TempData["error"]);
                }
            }
            if (role == "Am")
            {
                if (leaves.mSatus == (int)RequestEnum.Request.Approval || leaves.mSatus == (int)RequestEnum.Request.Reject)
                {
                    leaves.amRession = reason;
                    leaves.amSatus = (int)RequestEnum.Request.Approval;
                }
                else
                {
                    return BadRequest("manager not Take any action on it !!!");
                }
            }
            if (role == "Gm")
            {
                if (leaves.amSatus == (int)RequestEnum.Request.Approval || leaves.amSatus == (int)RequestEnum.Request.Reject)
                {
                    leaves.gmRession = reason;
                    leaves.gmSatus = (int)RequestEnum.Request.Approval;
                }
                else
                {
                    
                    return BadRequest("Assiestent Manager not Take any action on it !!!");
                }
            }
            if (role == "Tl")
            {
                leaves.tlRession = reason;
                leaves.tlSatus = (int)RequestEnum.Request.Approval;
            }
             

            var data1 = JsonConvert.SerializeObject(leaves);
                StringContent content = new StringContent(data1, Encoding.UTF8, "application/Json");
                HttpResponseMessage response2 = client.PutAsync(url + "?id=" + leaves.leaveId, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["Edit"] = "Approved Sucessfully";
                    if(role != "Hr")
                    {
                        return RedirectToAction("showLeave", "Leave");
                    }
                     else
                     {
                         return RedirectToAction("ShowAllLeaves", "Leave");
                     }
                }
            
            

            return RedirectToAction("showLeave", "Leave");
        }

        [HttpPost]
        public async Task<IActionResult> Rejected(int id,string reason )
        {
            LeaveRequest leaves = new LeaveRequest();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            HttpResponseMessage response = client.GetAsync(url + "details?id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<LeaveRequest>(result);
                if (data != null)
                {
                    leaves = data;
                }
            }
            if (role == "Hr")
            {
                if (leaves.gmSatus == (int)RequestEnum.Request.Approval || leaves.gmSatus == (int)RequestEnum.Request.Reject)
                {
                    leaves.hrSatus = (int)RequestEnum.Request.Reject;
                }
                else
                {
                    return BadRequest("Team Leader not Take any action on it !!!");
                }
            }
            if (role == "Manager")
            {
                if (leaves.tlSatus == (int)RequestEnum.Request.Approval || leaves.tlSatus == (int)RequestEnum.Request.Reject)
                {
                    leaves.mRession = reason;
                    leaves.mSatus = (int)RequestEnum.Request.Reject;
                }
                else
                {
                    return BadRequest("Team Leader not Take any action on it !!!");
                }
            }
            if (role == "Am")
            {
                if (leaves.mSatus == (int)RequestEnum.Request.Approval || leaves.mSatus == (int)RequestEnum.Request.Reject)
                {
                    leaves.amRession = reason;
                    leaves.amSatus = (int)RequestEnum.Request.Reject;
                }
                else
                { 
                    return BadRequest("manager not Take any action on it !!!");
                }
            }
            if (role == "Gm")
            {
                if (leaves.amSatus == (int)RequestEnum.Request.Approval || leaves.amSatus == (int)RequestEnum.Request.Reject)
                {
                    leaves.gmRession = reason;
                    leaves.gmSatus = (int)RequestEnum.Request.Reject;
                }
                else
                {
                    
                    return BadRequest("Assiestent Manager not Take any action on it !!!");
                }
            }
            if (role == "Tl")
            {
                leaves.tlRession = reason;
                leaves.tlSatus = (int)RequestEnum.Request.Reject;
            }


            var data1 = JsonConvert.SerializeObject(leaves);
            StringContent content = new StringContent(data1, Encoding.UTF8, "application/Json");
            HttpResponseMessage response2 = client.PutAsync(url + "?id=" + leaves.leaveId, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Edit"] = "Updated Sucessfully";
                if (role != "Hr")
                {
                    return RedirectToAction("showLeave", "Leave");
                }
                else
                {
                    return RedirectToAction("ShowAllLeaves", "Leave");
                }
                
            }



            return RedirectToAction("showLeave", "Leave");
        }

        [HttpGet]
        public IActionResult GetUserLeave() 
        {
            List<LeaveRequestModel> leave = new List<LeaveRequestModel>();
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                

                HttpResponseMessage response = client.GetAsync(url + "LeaveDetails?id=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<List<LeaveRequestModel>>(result);
                    if (data != null)
                    {
                        leave = data;
                        
                    }
                
            }
            return View(leave);


        }
    }
}
