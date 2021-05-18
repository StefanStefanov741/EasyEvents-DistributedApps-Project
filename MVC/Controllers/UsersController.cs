using ApplicationService.DTOs;
using MVC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
            if (ModelState.IsValid)
            {
                bool allowed = true;
                int exists_code = 200;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string safe_username = model.username.Replace(".","4242424242424242");
                    HttpResponseMessage response = await client.GetAsync("findbyusername/" + safe_username);
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var usernameResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                    int rd1 = Convert.ToInt32(usernameResponseData.Code);

                    string safe_email = model.email.Replace(".", "4242424242424242");
                    response = await client.GetAsync("findbyemail/" + safe_email);
                    jsonString = await response.Content.ReadAsStringAsync();
                    var emailResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                    int rd2 = Convert.ToInt32(emailResponseData.Code);

                    string safe_dispn = model.displayName.Replace(".", "4242424242424242");
                    response = await client.GetAsync("findbydispn/" + safe_dispn);
                    jsonString = await response.Content.ReadAsStringAsync();
                    var dispnameResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                    int rd3 = Convert.ToInt32(dispnameResponseData.Code);

                    if (rd1 == exists_code) {
                        allowed = false;
                        ModelState.AddModelError("RegistrationError", "Username taken!");
                        return View();
                    }
                    if (rd3 == exists_code)
                    {
                        allowed = false;
                        ModelState.AddModelError("RegistrationError", "Display name taken!");
                        return View();
                    }
                    if (rd2 == exists_code)
                    {
                        allowed = false;
                        ModelState.AddModelError("RegistrationError", "Email already registered!");
                        return View();
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

                HttpResponseMessage response = await client.GetAsync("trylogin/"+model.username+"/"+model.password);

                var content = JsonConvert.SerializeObject(model);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                string jsonString = await response.Content.ReadAsStringAsync();
                var LoginResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                int code = Convert.ToInt32(LoginResponseData.Code);
                if (code == 200)
                {
                    //create token
                    HttpResponseMessage token_response = await client.PostAsync("auth/"+model.username+"/"+model.password, byteContent);
                    string jsonTokenString = await token_response.Content.ReadAsStringAsync();
                    var TokenResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonTokenString);
                    HttpCookie JWTCookie = new HttpCookie("jwt");
                    JWTCookie.Value = TokenResponseData.Body.ToString();
                    JWTCookie.Expires = DateTime.Now.AddHours(2);
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
    }
}