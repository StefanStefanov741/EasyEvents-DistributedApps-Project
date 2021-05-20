using ApplicationService.DTOs;
using MVC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAPI.Messages;

namespace MVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly Uri url = new Uri("https://localhost:44368/api/users/");

        public ActionResult Register()
        {
            ViewData["loggedin"] = false;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register (RegisterVM model)
        {
            ViewData["loggedin"] = false;
            var genderValue = Request["GenderSelect"];
            if (genderValue == "Male")
            {
                model.gender = true;
            }
            else {
                model.gender = false;
            }
            if (ModelState.IsValid)
            {
                bool allowed = true;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    UserDTO userInfo = new UserDTO()
                    {
                        username = model.username,
                        email = model.email,
                        displayName = model.displayName
                    };

                    var content = JsonConvert.SerializeObject(userInfo);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("findexistinguser", byteContent);

                    string jsonString = await response.Content.ReadAsStringAsync();
                    var ExistingResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                    string res_body = (string)ExistingResponseData.Body;

                    if (res_body.Contains("Username exists!")) {
                        allowed = false;
                        ModelState.AddModelError("RegistrationError1", "Username taken!");
                    }
                    if (res_body.Contains("Displayname exists!"))
                    {
                        allowed = false;
                        ModelState.AddModelError("RegistrationError2", "Display name taken!");
                    }
                    if (res_body.Contains("Email exists!"))
                    {
                        allowed = false;
                        ModelState.AddModelError("RegistrationError3", "Email already registered!");
                    }
                }
                if (allowed)
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = url;
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            UserDTO toRegister = new UserDTO()
                            {
                                username = model.username,
                                password = model.password,
                                displayName = model.displayName,
                                email = model.email,
                                birthday = model.birthday,
                                gender = model.gender
                            };

                            var content = JsonConvert.SerializeObject(toRegister);
                            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                            var byteContent = new ByteArrayContent(buffer);
                            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            HttpResponseMessage response = await client.PostAsync("create", byteContent);
                            return RedirectToAction("Login", "Users");
                        }
                    }
                    catch
                    {
                        return View();
                    }
                }
                else {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Login()
        {
            ViewData["loggedin"] = false;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginVM model)
        {
            ViewData["loggedin"] = false;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = JsonConvert.SerializeObject(model);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync("trylogin", byteContent);

                string jsonString = await response.Content.ReadAsStringAsync();
                var LoginResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                int code = Convert.ToInt32(LoginResponseData.Code);
                if (code == 200)
                {
                    UserDTO u = new UserDTO() { username = model.username, password = model.password };
                    //create token
                    var token_content = JsonConvert.SerializeObject(u);
                    var token_buffer = System.Text.Encoding.UTF8.GetBytes(token_content);
                    var token_byteContent = new ByteArrayContent(token_buffer);
                    token_byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage token_response = await client.PostAsync("auth", token_byteContent);
                    string jsonTokenString = await token_response.Content.ReadAsStringAsync();
                    var TokenResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonTokenString);
                    HttpCookie JWTCookie = new HttpCookie("jwt");
                    JWTCookie.Value = TokenResponseData.Body.ToString();
                    JWTCookie.Expires = DateTime.Now.AddHours(2);
                    JWTCookie.HttpOnly = true;
                    Response.Cookies.Add(JWTCookie);
                    return RedirectToAction("Index", "Home");
                }
                else {
                    //return error
                    ModelState.AddModelError("AuthenticationFailed", "Wrong username or password!");
                    return View();
                }
            }
        }
        public ActionResult Logout()
        {
            HttpCookie oldCookie = new HttpCookie("jwt");
            oldCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(oldCookie);
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Profile() {
            UserDTO user = new UserDTO();
            ProfileVM model = new ProfileVM();
            if (HttpContext.Request.Cookies["jwt"] == null)
            {
                ViewData["loggedin"] = false;
            }
            else
            {
                ViewData["loggedin"] = true;
                using (var client = new HttpClient())
                {
                    HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");

                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(jwtCookie);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("userfromtoken", byteContent);
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var UserResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                    string username = "";
                    try
                    {
                        username = UserResponseData.Body.ToString();
                    }
                    catch
                    {
                        //someone tampered with the token
                        ViewData["loggedin"] = false;
                        HttpCookie oldCookie = new HttpCookie("jwt");
                        oldCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(oldCookie);
                        return View();
                    }
                    if (username != "")
                    {
                        UserDTO u = new UserDTO() { username = username };
                        var content2 = JsonConvert.SerializeObject(u);
                        var buffer2 = System.Text.Encoding.UTF8.GetBytes(content2);
                        var byteContent2 = new ByteArrayContent(buffer2);
                        byteContent2.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        HttpResponseMessage user_response = await client.PostAsync("getbyusername", byteContent2);
                        var User_jsonString = await user_response.Content.ReadAsStringAsync();
                        var UserResoponseData = JsonConvert.DeserializeObject<UserDTO>(User_jsonString);
                        user = UserResoponseData;
                        ViewData["DisplayName"] = user.displayName;
                    }
                    else
                    {
                        ViewData["loggedin"] = false;
                    }
                }
            }
            if (user != null) {
                //add user to model and pass the model to the view
                model = new ProfileVM(user);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Profile(ProfileVM model) {
            if (ModelState.IsValid)
            {
                var genderValue = Request["GenderSelect"];
                if (genderValue == "Male")
                {
                    model.gender = true;
                }
                else
                {
                    model.gender = false;
                }
                using (var client = new HttpClient())
                {
                    HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");

                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(model);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("updateuser", byteContent);

                    //remove old token since there might have been changes to the username or password
                    HttpCookie oldCookie = new HttpCookie("jwt");
                    oldCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(oldCookie);
                    //create new token

                    UserDTO u = new UserDTO() { username = model.username, password = model.password };

                    //create token
                    var token_content = JsonConvert.SerializeObject(u);
                    var token_buffer = System.Text.Encoding.UTF8.GetBytes(token_content);
                    var token_byteContent = new ByteArrayContent(token_buffer);
                    token_byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage token_response = await client.PostAsync("auth", token_byteContent);
                    string jsonTokenString = await token_response.Content.ReadAsStringAsync();
                    var TokenResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonTokenString);
                    HttpCookie JWTCookie = new HttpCookie("jwt");
                    JWTCookie.Value = TokenResponseData.Body.ToString();
                    JWTCookie.Expires = DateTime.Now.AddHours(2);
                    JWTCookie.HttpOnly = true;
                    Response.Cookies.Add(JWTCookie);
                }

                return RedirectToAction("Index", "Home");
            }
            else {
                ViewData["loggedin"] = true;
                using (var client = new HttpClient())
                {
                    HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");

                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(jwtCookie);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("userfromtoken", byteContent);
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var UserResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                    string username = "";
                    try
                    {
                        username = UserResponseData.Body.ToString();
                    }
                    catch
                    {
                        //someone tampered with the token
                        ViewData["loggedin"] = false;
                        HttpCookie oldCookie = new HttpCookie("jwt");
                        oldCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(oldCookie);
                        return View();
                    }
                    if (username != "")
                    {
                        ViewData["DisplayName"] = username;
                    }
                    else
                    {
                        ViewData["loggedin"] = false;
                    }
                }
                return View(model);
            }
        }


    }
}