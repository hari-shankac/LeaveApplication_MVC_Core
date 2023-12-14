using LeaveApplication_MVC_Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace LeaveApplication_MVC_Core.Controllers
{
    public class AccountController : Controller
    {
        private string url = "https://localhost:7183/api/Account/";
        HttpClient _client = new HttpClient();


        public ActionResult Registor()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Hr")]
        public async Task<ActionResult<RegistorModel>> Registor(RegistorModel registorModel)
        {
            var data = JsonConvert.SerializeObject(registorModel);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/Json");

            HttpResponseMessage response = _client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["add"] = " Welcome ! User added Sucessfully !! ";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/Json");

                HttpResponseMessage response = _client.PostAsync(url + "Login", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string token = response.Content.ReadAsStringAsync().Result;

                    if (token == "User not exist")
                    {
                        //ModelState.AddModelError("", "Enter valid email and password");
                        TempData["Invalid"] = "Invalid User";
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                        string role = jwt.Claims.ToList()[1].Value;
                        HttpContext.Session.SetString("userToken", token);

                        var claimsIdentity = new ClaimsIdentity(jwt.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                        if (role == "Employee")
                        {
                            TempData["enter"] = "hello User";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (role == "Gm")
                        {
                            TempData["enter"] = "hello Generalmanager";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (role == "Am")
                        {
                            TempData["enter"] = "hello Assiestent manager";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (role == "Tl")
                        {
                            TempData["enter"] = "hello Team Leader";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (role == "Hr")
                        {
                            TempData["enter"] = "hello hr";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (role == "Manager")
                        {
                            TempData["enter"] = "hello manager";
                            return RedirectToAction("Index", "Home");
                        }
                    }

                }
                else
                {
                    return RedirectToAction("Login");
                }

                TempData["Login"] = "Login Successfully !!!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Handle the case where the login request failed
                ModelState.AddModelError("", "Login failed. Please try again.");
                return View();
            }

        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            ResetPasswordDto model = new ResetPasswordDto { token = token, Email = email };
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent postData = new StringContent(data, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7183");
                var resp = await client.PostAsync("/api/Account/ResetPassword", postData);
                var status = resp;
                if (status.IsSuccessStatusCode)
                {
                    var tempToken = await status.Content.ReadAsStringAsync();
                    var token = tempToken;
                    if (token == "User Doesn't Exist!")
                    {

                        ModelState.AddModelError("", "User Doesn't Exist!");
                        return View();
                    }
                    else
                    {
                        TempData["Password_reset"] = "Password Reset Successful...";
                        return RedirectToAction("Login");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Internal Server Error. Please try after some time!!");
                return View();
            }
            return View();
        }

        [HttpGet]
        public IActionResult DeleteUser(string id)
        {
            var user = new Employee();
            HttpResponseMessage response = _client.GetAsync(url + "UserDetails?id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<Employee>(result);
                if (data != null)
                {
                    user = data;
                }
                return View(user);

            }
            return View();
        }
        

         [HttpPost]
        public IActionResult DeleteUser(string Id, int test)
        {
            HttpResponseMessage response = _client.GetAsync(url + "DeleteUser?id=" + Id).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["delete"] = "User Deleted Sucessfully !!!";
                return RedirectToAction("", "");

            }
            return View();


        }

        [HttpGet]
        [Authorize(Roles ="Hr")]
        public async Task<IActionResult> AllUser()
        {
            var users = new List<Employee>();
            HttpResponseMessage response = _client.GetAsync(url + "AllUsers").Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<List<Employee>>(result);
                if (data != null)
                {
                    users = data;
                    return View(users);
                }
            }
            return View();

        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = new Employee();
            HttpResponseMessage response = _client.GetAsync(url + "UserDetails?id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<Employee>(result);
                if (data != null)
                {
                    user = data;
                }
                return View(user);

            }
            return View();
        }

        [HttpGet]
        public IActionResult EditUser(string id)
        {
            var user = new Employee();
            HttpResponseMessage response = _client.GetAsync(url + "UserDetails?id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<Employee>(result);
                if (data != null)
                {
                    user = data;
                }
                return View(user);

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string id, Employee user)
        {
            if (ModelState.IsValid)
            {
                var data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/Json");
                HttpResponseMessage response = _client.PutAsync(url + "?id=" + user.Id, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["Edit"] = "Updated Sucessfully";
                    return RedirectToAction("AllUser", "Account");
                }
            }
            return View();
        }
    }
}
