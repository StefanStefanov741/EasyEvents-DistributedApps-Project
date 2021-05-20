using ApplicationService.DTOs;
using MVC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAPI.Messages;

namespace MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly Uri url = new Uri("https://localhost:44368/api/events/");
        public async Task<ActionResult> Index()
        {
            //test if user is still logged in and authorized
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            //check if user is logged in or not
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
            }
            else
            {
                string displayn = await GetDisplayName(jwtCookie);
                if (displayn == "user_not_logged_in_false_login_viewdata")
                {
                    ViewData["loggedin"] = false;
                }
                else {
                    ViewData["loggedin"] = true;
                    ViewData["DisplayName"] = displayn;
                }
            }

            //get events
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<List<EventListVM>>(jsonString);
                return View(responseData);
            }
        }

        public async Task<ActionResult> Create() {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
            }
            else {
                string displayn = await GetDisplayName(jwtCookie);
                if (displayn == "user_not_logged_in_false_login_viewdata")
                {
                    ViewData["loggedin"] = false;
                    RedirectToAction("Login", "Users");
                }
                else
                {
                    ViewData["loggedin"] = true;
                    ViewData["DisplayName"] = displayn;
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(EventVM model)
        {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
            }
            else
            {
                string displayn = await GetDisplayName(jwtCookie);
                if (displayn == "user_not_logged_in_false_login_viewdata")
                {
                    ViewData["loggedin"] = false;
                    RedirectToAction("Login", "Users");
                }
                else
                {
                    ViewData["loggedin"] = true;
                    ViewData["DisplayName"] = displayn;
                }
            }
            //start creating event
            if (ModelState.IsValid)
            {
                int user_id = await GetUserId();
                if (user_id == 0) {
                    ViewData["loggedin"] = false;
                    HttpCookie oldCookie = new HttpCookie("jwt");
                    oldCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(oldCookie);
                    return RedirectToAction("Login", "Users");
                }
                int h_id = await GetUserId();
                EventDTO new_event = new EventDTO()
                {
                    title = model.title,
                    description = model.description,
                    location = model.location,
                    host_id = h_id,
                    likes = 0,
                    createdOn = DateTime.Now,
                    begins = new DateTime(model.begins_date.Date.Year, model.begins_date.Date.Month, model.begins_date.Date.Day, model.begins_time.Hour, model.begins_time.Minute, 0),
                    ends = new DateTime(model.ends_date.Date.Year, model.ends_date.Date.Month, model.ends_date.Date.Day, model.ends_time.Hour, model.ends_time.Minute, 0),
                    participants = 1
                };
                if (new_event.begins > new_event.ends) {
                    ModelState.AddModelError("InvalidDates", "The begins date needs to be before the ends date :)");
                    return View(model);
                }
                if (new_event.begins < DateTime.Now)
                {
                    ModelState.AddModelError("InvalidDates", "Create an event in the future, not the past :)");
                    return View(model);
                }
                //save the event
                using (var client = new HttpClient())
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(new_event);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("create", byteContent);
                }
                //get event id
                int ev_id = 0;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(new_event);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("getid", byteContent);
                    string jsonString = await response.Content.ReadAsStringAsync();
                    ev_id = JsonConvert.DeserializeObject<int>(jsonString);
                }
                //save connection between host and event
                HostToEventDTO new_hte = new HostToEventDTO()
                {
                    Host_id = h_id,
                    Event_id = ev_id
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44368/api/hte/"); ;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(new_hte);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("create", byteContent);
                }
                //save connection between participant and event
                ParticipantToEventDTO new_pte = new ParticipantToEventDTO()
                {
                    Participant_id = h_id,
                    Event_id = ev_id
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44368/api/pte/"); ;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(new_pte);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("create", byteContent);
                }
                return RedirectToAction("Index", "Events");
            }
            else {
                return View(model);
            }
        }
        async Task<int> GetUserId(){
            int id = 0;
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            if (jwtCookie == null)
            {
                return 0;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/users/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(jwtCookie);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("userfromtoken", byteContent);
                string jsonString = await response.Content.ReadAsStringAsync();
                var ResponseData = JsonConvert.DeserializeObject<ResponseMessage>(jsonString);
                string username = "";
                try
                {
                    username = ResponseData.Body.ToString();
                }
                catch
                {
                    //someone tampered with the token
                    HttpCookie oldCookie = new HttpCookie("jwt");
                    oldCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(oldCookie);
                    return 0;
                }
                if (username != "")
                {
                    UserDTO u = new UserDTO() { username = username };
                    var user_content = JsonConvert.SerializeObject(u);
                    var user_buffer = System.Text.Encoding.UTF8.GetBytes(user_content);
                    var user_byteContent = new ByteArrayContent(user_buffer);
                    user_byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage user_response = await client.PostAsync("getbyusername", user_byteContent);
                    string user_jsonString = await user_response.Content.ReadAsStringAsync();
                    var UserResponseData = JsonConvert.DeserializeObject<UserDTO>(user_jsonString);
                    id = UserResponseData.Id;
                }
                else
                {
                    return 0;
                }
            }
            return id;
        }
        async Task<string> GetDisplayName(HttpCookie jwtCookie) {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/users/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //adding jw token to the request headers since the method checks for authorization
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
                    HttpCookie oldCookie = new HttpCookie("jwt");
                    oldCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(oldCookie);
                    return "user_not_logged_in_false_login_viewdata";
                }
                if (username != "")
                {
                    UserDTO u = new UserDTO() { username = username };
                    var content2 = JsonConvert.SerializeObject(u);
                    var buffer2 = System.Text.Encoding.UTF8.GetBytes(content2);
                    var byteContent2 = new ByteArrayContent(buffer2);
                    byteContent2.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage displayNresponse = await client.PostAsync("getbyusername",byteContent2);
                    string jsonStringDname = await displayNresponse.Content.ReadAsStringAsync();
                    var DnameData = JsonConvert.DeserializeObject<UserDTO>(jsonStringDname);
                    return DnameData.displayName;
                }
                else
                {
                    return "user_not_logged_in_false_login_viewdata";
                }
            }
        }

    }
}