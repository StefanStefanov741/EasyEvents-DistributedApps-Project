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

        public async Task<ActionResult> Index(string sn,string sr) {
            if (sn != null)
            {
                ViewData["snVal"] = sn;
            }
            else
            {
                ViewData["snVal"] = "";
            }
            if (sr != null)
            {
                ViewData["srVal"] = sr.ToString();
            }
            else
            {
                ViewData["srVal"] = "";
            }
            //test if user is still logged in and authorized
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            //check if user is logged in or not
            UserDTO current_user = null;
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login","Users");
            }
            else
            {
                ViewData["loggedin"] = true;
                using (var client = new HttpClient())
                {
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
                    current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                }
                ViewData["DisplayName"] = current_user.displayName;
            }
            //get all users
            List<UsersListVM> users;
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response;
                var isNumberic = int.TryParse(sr, out _);
                if (!isNumberic)
                {
                    sr = "";
                }
                if ((sn == "" || sn == null) && (sr == "" || sr == null))
                {
                    response = await client.GetAsync("all");
                }
                else
                {
                    if (sn == null || sn == "")
                    {
                        response = await client.GetAsync("allsr/" + sr);
                    }
                    else if (sr == null || sr == "")
                    {
                        response = await client.GetAsync("allsn/" + sn);
                    }
                    else
                    {
                        response = await client.GetAsync("all/" + sn + "/" + sr);
                    }
                }

                string jsonString = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<UsersListVM>>(jsonString);
            }
            int removeCurrentUserAt = -1;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].id == current_user.Id) {
                    removeCurrentUserAt = i;
                }
                if (users[i].gender == "true")
                {
                    users[i].gender = "Male";
                }
                else
                {
                    users[i].gender = "Female";
                }
                users[i].sentRequest = false;
            }
            if (removeCurrentUserAt > -1) {
                users.RemoveAt(removeCurrentUserAt);
            }
            //get all friends ids
            List<FriendshipDTO> existing_friendships = new List<FriendshipDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/friends/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                existing_friendships = JsonConvert.DeserializeObject<List<FriendshipDTO>>(jsonString);
            }
            //remove option to add friends if users are already friends
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = 0; j < existing_friendships.Count; j++)
                {
                    if ((users[i].id == existing_friendships[j].user1_id && current_user.Id == existing_friendships[j].user2_id) || (users[i].id == existing_friendships[j].user2_id && current_user.Id == existing_friendships[j].user1_id)) {
                        users[i].sentRequest = true;
                    }
                }
            }
            return View(users);
        }

        public ActionResult SendRequest(int id) {
            return RedirectToAction("SendRequest", "Friends", new { id = id});
        }

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

        public async Task<ActionResult> ViewUserProfile(int id) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            UserDTO user = new UserDTO();
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
            }
            else {
                ViewData["loggedin"] = true;
                using (var client = new HttpClient())
                {
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
                    var UserResponseData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                    string username = "";
                    try
                    {
                        username = UserResponseData.username;
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
            UserDetailsVM model = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/users/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("getbyid/"+id);

                string jsonString = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<UserDetailsVM>(jsonString);
                if (model.gender=="true")
                {
                    model.gender = "Male";
                }
                else
                {
                    model.gender = "Female";
                }
            }
            List<FriendshipDTO> friends = new List<FriendshipDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/friends/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var fr_conns = JsonConvert.DeserializeObject<List<FriendshipDTO>>(jsonString);
                for (int i = 0; i < fr_conns.Count; i++)
                {
                    if (fr_conns[i].user1_id == user.Id || fr_conns[i].user2_id == user.Id)
                    {
                        friends.Add(fr_conns[i]);
                    }
                }
            }
            for (int i = 0; i < friends.Count; i++)
            {
                if (friends[i].user1_id == model.Id || friends[i].user2_id == model.Id) {
                    if (friends[i].pending)
                    {
                        model.sentrequest = true;
                    }
                    else {
                        model.friends = true;
                        model.sentrequest = true;
                    }
                }
            }
            return View(model);
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
                    var UserResponseData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                    string username = "";
                    try
                    {
                        username = UserResponseData.username;
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
                    var UserResponseData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                    string username = "";
                    try
                    {
                        username = UserResponseData.username;
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

        public async Task<ActionResult> Delete(int id) {
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            List<int> participatedInEventsIDS = new List<int>();
            //delete participation connections
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri("https://localhost:44368/api/pte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                var ptes = JsonConvert.DeserializeObject<List<ParticipantToEventDTO>>(jsonString);
                for (int i = 0; i < ptes.Count; i++)
                {
                    if (ptes[i].Participant_id == id) {
                        participatedInEventsIDS.Add(ptes[i].Event_id);
                        await client.DeleteAsync("" + ptes[i].Id);
                    }
                }
            }
            //delete host connections
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
                    if (htes[i].Host_id == id)
                    {
                        await client.DeleteAsync("" + htes[i].Id);
                    }
                }
            }
            //delete likes
            List<int> likedEventsIDS = new List<int>();
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
                    if (likes[i].User_id == id)
                    {
                        likedEventsIDS.Add(likes[i].Event_id);
                        await client.DeleteAsync("" + likes[i].Id);
                    }
                }
            }
            //delete hosted events
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/events/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<List<EventDTO>>(jsonString);
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].host_id == id)
                    {
                        await client.DeleteAsync("" + events[i].Id);
                    }
                    else {
                        //decrease likes and participations on events
                        bool update = false;
                        if (likedEventsIDS.Contains(events[i].Id)) {
                            events[i].likes--;
                            update = true;
                        }
                        if (participatedInEventsIDS.Contains(events[i].Id))
                        {
                            events[i].participants--;
                            update = true;
                        }
                        if (update) {
                            var content = JsonConvert.SerializeObject(events[i]);
                            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                            var byteContent = new ByteArrayContent(buffer);
                            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            await client.PostAsync("updateevent", byteContent);
                        }
                    }
                }
            }
            //delete friendships
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/friends/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                var friendships = JsonConvert.DeserializeObject<List<FriendshipDTO>>(jsonString);
                for (int i = 0; i < friendships.Count; i++)
                {
                    if (friendships[i].user1_id == id || friendships[i].user2_id == id)
                    {
                        await client.DeleteAsync("" + friendships[i].Id);
                    }
                }
            }
            //delete user
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                await client.DeleteAsync(""+id);
            }
            //log out
            HttpCookie oldCookie = new HttpCookie("jwt");
            oldCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(oldCookie);
            ViewData["loggedin"] = false;
            return RedirectToAction("Index","Home");
        }

        public async Task<ActionResult> UserHostedEvents(int id) {
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
                    current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                }
                ViewData["DisplayName"] = current_user.displayName;
            }
            List<EventDTO> All_events;
            //get all events
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/events/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = null;
                response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                All_events = JsonConvert.DeserializeObject<List<EventDTO>>(jsonString);
            }
            if (current_user == null)
            {
                return RedirectToAction("Login", "Users");
            }
            //get events hosted by the user
            List<EventListVM> events = new List<EventListVM>();
            for (int i = 0; i < All_events.Count; i++)
            {
                if (All_events[i].host_id == id) {
                    events.Add(new EventListVM(All_events[i]));
                }
            }
            return View(events);
        }

        public async Task<ActionResult> UserJoinedEvents(int id)
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
                    current_user = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                }
                ViewData["DisplayName"] = current_user.displayName;
            }
            if (current_user == null)
            {
                return RedirectToAction("Login", "Users");
            }
            List<EventDTO> All_events;
            //get all events
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/events/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = null;
                response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                All_events = JsonConvert.DeserializeObject<List<EventDTO>>(jsonString);
            }
            //get pte's
            List<ParticipantToEventDTO> pteList = new List<ParticipantToEventDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/pte/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);
                HttpResponseMessage response = null;
                response = await client.GetAsync("all");
                string jsonString = await response.Content.ReadAsStringAsync();
                pteList = JsonConvert.DeserializeObject<List<ParticipantToEventDTO>>(jsonString);
            }
            List<EventListVM> events = new List<EventListVM>();
            for (int i = 0; i < All_events.Count; i++)
            {
                for (int j = 0; j < pteList.Count; j++)
                {
                    if (pteList[j].Event_id == All_events[i].Id && pteList[j].Participant_id == id) {
                        events.Add(new EventListVM(All_events[i]));
                    }
                }
            }
            return View(events);
        }

    }
}