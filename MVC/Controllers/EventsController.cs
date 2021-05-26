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
            UserDTO current_user = null;
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
            }
            else
            {
                ViewData["loggedin"] = true;
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
                    current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                }
                ViewData["DisplayName"] = current_user.displayName;
            }
            //get events
            List<EventListVM> events;
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                events = JsonConvert.DeserializeObject<List<EventListVM>>(jsonString);
            }
            if (current_user == null)
            {
                return View(events);
            }
            //get all likes
            List<LikeDTO> likes;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/likes/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                likes = JsonConvert.DeserializeObject<List<LikeDTO>>(jsonString);
            }
            for (int i = 0; i < events.Count; i++)
            {
                for (int j = 0; j < likes.Count; j++)
                {
                    if (likes[j].Event_id == events[i].id && likes[j].User_id == current_user.Id) {
                        events[i].likedByUser = true;
                    }
                }
            }
            return View(events);
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
                    begins = new DateTime(model.begins_date.Year, model.begins_date.Month, model.begins_date.Day, model.begins_time.Hour,model.begins_time.Minute,0),
                    ends = new DateTime(model.ends_date.Year, model.ends_date.Month, model.ends_date.Day, model.ends_time.Hour,model.ends_time.Minute,0),
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

        public async Task<ActionResult> Delete(int id) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
            }
            //get all participant connections and delete them
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/pte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                var ptes = JsonConvert.DeserializeObject<List<ParticipantToEventDTO>>(jsonString);
                for (int i = 0; i < ptes.Count; i++)
                {
                    if (ptes[i].Event_id == id) {
                        await client.DeleteAsync(""+ptes[i].Id);
                    }
                }
            }
            //get host connection and delete it
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/hte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                var htes = JsonConvert.DeserializeObject<List<ParticipantToEventDTO>>(jsonString);
                for (int i = 0; i < htes.Count; i++)
                {
                    if (htes[i].Event_id == id)
                    {
                        await client.DeleteAsync("" + htes[i].Id);
                        break;
                    }
                }
            }
            //delete likes
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/likes/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                var likes = JsonConvert.DeserializeObject<List<LikeDTO>>(jsonString);
                for (int i = 0; i < likes.Count; i++)
                {
                    if (likes[i].Event_id == id)
                    {
                        await client.DeleteAsync("" + likes[i].Id);
                    }
                }
            }
            //delete the event
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                await client.DeleteAsync("" + id);
            }
            return RedirectToAction("Index","Events");
        }
        public async Task<int> GetUserId(){
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
                var ResponseData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                string username = "";
                try
                {
                    username = ResponseData.username;
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
                var UserResponseData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                string username = "";
                try
                {
                    username = UserResponseData.username;
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

        public async Task<ActionResult> Like(int id) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            UserDTO userDto = null;
            if (jwtCookie == null) {
                return RedirectToAction("Login", "Users");
            }
            //get user that liked
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri("https://localhost:44368/api/users/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(jwtCookie);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage user_msg = await client.PostAsync("userfromtoken", byteContent);
                string jsonString = await user_msg.Content.ReadAsStringAsync();
                userDto = JsonConvert.DeserializeObject<UserDTO>(jsonString);
            }
            if (userDto == null) {
                return RedirectToAction("Login", "Users");
            }
            //check if user has already liked the event
            bool liked = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/likes/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);
                HttpResponseMessage likes_msg = await client.GetAsync("all");
                string jsonString = await likes_msg.Content.ReadAsStringAsync();
                var likes = JsonConvert.DeserializeObject<List<LikeDTO>>(jsonString);
                for (int i = 0; i < likes.Count; i++)
                {
                    if (likes[i].Event_id == id && likes[i].User_id == userDto.Id) {
                        liked = true;
                        break;
                    }
                }
            }
            if (liked) {
                return RedirectToAction("Index", "Events");
            }
            //create a new like and save it
            LikeDTO like = new LikeDTO() {
                User_id = userDto.Id,
                Event_id = id
            };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/likes/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(like);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage user_msg = await client.PostAsync("create", byteContent);
            }
            //increment the likes on the event
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(like);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage user_msg = await client.PostAsync("like", byteContent);
            }
            return RedirectToAction("Index", "Events");
        }

        public async Task<ActionResult> Dislike(int id) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            UserDTO userDto = null;
            if (jwtCookie == null)
            {
                return RedirectToAction("Login", "Users");
            }
            //get user that disliked
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
                HttpResponseMessage user_msg = await client.PostAsync("userfromtoken", byteContent);
                string jsonString = await user_msg.Content.ReadAsStringAsync();
                userDto = JsonConvert.DeserializeObject<UserDTO>(jsonString);
            }
            if (userDto == null)
            {
                return RedirectToAction("Login", "Users");
            }
            //find like
            int like_id = 0;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/likes/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var likes = JsonConvert.DeserializeObject<List<LikeDTO>>(jsonString);
                for (int i = 0; i < likes.Count; i++)
                {
                    if (likes[i].Event_id == id && likes[i].User_id == userDto.Id) {
                        like_id = likes[i].Id;
                        break;
                    }
                }
            }
            if (like_id == 0) {
                return RedirectToAction("Index", "Events");
            }
            //delete like
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/likes/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(jwtCookie);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage res_msg = await client.DeleteAsync(""+like_id);
            }
            //decrese likes on the event
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);
                EventDTO ev = new EventDTO() { Id = id };
                var content = JsonConvert.SerializeObject(ev);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage res_msg = await client.PostAsync("dislike", byteContent);
            }
            return RedirectToAction("Index", "Events");
        }

        public async Task<ActionResult> ViewDetails(int id) {
            //if user is not logged in prompt him to log in
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

            //get all events
            EventDTO eventDto = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<List<EventDTO>>(jsonString);
                //find event
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].Id == id) {
                        eventDto = events[i];
                        break;
                    }
                }
            }

            if (eventDto == null) {
                return RedirectToAction("Index", "Events");
            }

            //get host's name
            string host = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/users/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage user_msg = await client.GetAsync("getbyid/"+eventDto.host_id);
                string jsonString = await user_msg.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                host = user.displayName;
            }
            //get current user
            UserDTO current_user = null;
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
                current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
            }

            //get friends going count
            int friendsgoing = 0;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/friends/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage friends_request = await client.GetAsync("all");
                string jsonString = await friends_request.Content.ReadAsStringAsync();
                var friendships = JsonConvert.DeserializeObject<List<FriendshipDTO>>(jsonString);
                for (int i = 0; i < friendships.Count; i++)
                {
                    if (friendships[i].user1_id == current_user.Id || friendships[i].user2_id == current_user.Id) {
                        if (friendships[i].pending == false) {
                            friendsgoing++;
                        }
                    }
                }
            }
            //check if current user has joined the event already
            bool joined = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/pte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage friends_request = await client.GetAsync("all");
                string jsonString = await friends_request.Content.ReadAsStringAsync();
                var participants = JsonConvert.DeserializeObject<List<ParticipantToEventDTO>>(jsonString);
                for (int i = 0; i < participants.Count; i++)
                {
                    if (participants[i].Event_id == id && participants[i].Participant_id == current_user.Id) {
                        joined = true;
                    }
                }
            }
            bool hosting = false;
            if (eventDto.host_id == current_user.Id) {
                hosting = true;
            }
            //create a model
            EventDetailsVM model = new EventDetailsVM(eventDto);
            model.host_name = host;
            model.friendsgoing = friendsgoing;
            model.joined = joined;
            model.hosting = hosting;
            return View(model);
        }

        public async Task<ActionResult> JoinEvent(int id) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            //check if user is logged in
            if (jwtCookie == null) {
                return RedirectToAction("Login", "Users");
            }
            //get the event
            EventDTO eventDto = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<List<EventDTO>>(jsonString);
                //find event
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].Id == id)
                    {
                        eventDto = events[i];
                        break;
                    }
                }
            }
            //get the user
            UserDTO current_user = null;
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
                current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
            }

            //create participate connection
            ParticipantToEventDTO pteDto = new ParticipantToEventDTO()
            {
                Participant_id = current_user.Id,
                Event_id = eventDto.Id
            };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/pte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(pteDto);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("create", byteContent);
            }
            //increment participants of the event
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(pteDto);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("joinevent", byteContent);
            }

            return RedirectToAction("ViewDetails", "Events", new { id = id });
        }

        public async Task<ActionResult> LeaveEvent(int id)
        {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            UserDTO userDto = null;
            if (jwtCookie == null)
            {
                return RedirectToAction("Login", "Users");
            }
            //get user that wants to leave event
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
                HttpResponseMessage user_msg = await client.PostAsync("userfromtoken", byteContent);
                string jsonString = await user_msg.Content.ReadAsStringAsync();
                userDto = JsonConvert.DeserializeObject<UserDTO>(jsonString);
            }
            if (userDto == null)
            {
                return RedirectToAction("Login", "Users");
            }
            //find pte
            int pte_id = 0;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/pte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var ptes = JsonConvert.DeserializeObject<List<ParticipantToEventDTO>>(jsonString);
                for (int i = 0; i < ptes.Count; i++)
                {
                    if (ptes[i].Event_id == id && ptes[i].Participant_id == userDto.Id)
                    {
                        pte_id = ptes[i].Id;
                        break;
                    }
                }
            }
            if (pte_id == 0)
            {
                return RedirectToAction("Index", "Events");
            }
            //delete pte
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/pte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(jwtCookie);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage res_msg = await client.DeleteAsync("" + pte_id);
            }
            //decrese participants of the event
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);
                EventDTO ev = new EventDTO() { Id = id };
                var content = JsonConvert.SerializeObject(ev);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage res_msg = await client.PostAsync("leaveevent", byteContent);
            }
            return RedirectToAction("ViewDetails", "Events", new { id = id });
        }

        public async Task<ActionResult> EditEvent(int id) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            UserDTO current_user = null;
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
            }
            else {
                ViewData["loggedin"] = true;
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
                    current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                }
                ViewData["DisplayName"] = current_user.displayName;
            }
            //check if current user is the host of the event
            HostToEventDTO hte = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/hte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var htes = JsonConvert.DeserializeObject<List<HostToEventDTO>>(jsonString);

                for (int i = 0; i < htes.Count; i++)
                {
                    if (htes[i].Host_id == current_user.Id && htes[i].Event_id == id) {
                        hte = htes[i];
                        break;
                    }
                }
            }
            if (hte == null) {
                return RedirectToAction("Index", "Events");
            }
            //get event dto
            EventDTO ev = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<List<EventDTO>>(jsonString);

                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].Id == id)
                    {
                        ev = events[i];
                        break;
                    }
                }
            }
            if (ev == null) {
                return RedirectToAction("Index", "Events");
            }
            EventVM model = new EventVM(ev);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditEvent(EventVM model) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
            }
            else {
                ViewData["loggedin"] = true;
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
                    var current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                    ViewData["DisplayName"] = current_user.displayName;
                }
            }
            if (ModelState.IsValid)
            {
                EventDTO eventDto = new EventDTO()
                {
                    Id = model.id,
                    title = model.title,
                    description = model.description,
                    location = model.location,
                    host_id = model.host_id,
                    likes = model.likes,
                    createdOn = model.createdOn,
                    begins = new DateTime(model.begins_date.Year, model.begins_date.Month, model.begins_date.Day, model.begins_time.Hour, model.begins_time.Minute, 0),
                    ends = new DateTime(model.ends_date.Year, model.ends_date.Month, model.ends_date.Day, model.ends_time.Hour, model.ends_time.Minute, 0),
                    participants = model.participants
                };
                //update
                using (var client = new HttpClient()) {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                    var content = JsonConvert.SerializeObject(eventDto);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync("updateevent", byteContent);
                }
                return RedirectToAction("ViewDetails", "Events", new { id = eventDto.Id });
            }
            else {
                return View(model);
            }
        }

    }
}