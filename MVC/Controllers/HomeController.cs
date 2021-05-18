using ApplicationService.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using WebAPI.Messages;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly Uri url = new Uri("https://localhost:44368/api/users/");
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            if (HttpContext.Request.Cookies["jwt"] == null)
            {
                ViewData["loggedin"] = false;
            }
            else {
                string displayName = "";
                ViewData["loggedin"] = true;
                using (var client = new HttpClient()) {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");

                    var content = JsonConvert.SerializeObject(jwtCookie);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("userfromtoken", byteContent);
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var LoginResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                    string username = LoginResponseData.Body.ToString();
                    if (username != "")
                    {
                        HttpResponseMessage displayNresponse = await client.GetAsync("getbyusername/" + username);
                        string jsonStringDname = await displayNresponse.Content.ReadAsStringAsync();
                        var DnameData = JsonConvert.DeserializeObject<UserDTO>(jsonStringDname);
                        ViewData["DisplayName"] = DnameData.displayName;
                    }
                    else {
                        ViewData["loggedin"] = false;
                    }
                }
            }
            return View();
        }
    }
}