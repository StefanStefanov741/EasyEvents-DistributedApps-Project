using ApplicationService.DTOs;
using MVC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class FriendsController : Controller
    {
        private readonly Uri url = new Uri("https://localhost:44368/api/friends/");
        public async System.Threading.Tasks.Task<ActionResult> Index(string sn,string se,string sr)
        {
            if (sn != null)
            {
                ViewData["snVal"] = sn;
            }
            else
            {
                ViewData["snVal"] = "";
            }
            if (se != null)
            {
                ViewData["seVal"] = se;
            }
            else
            {
                ViewData["seVal"] = "";
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
            //get all friendships from db
            List<FriendshipDTO> friendships;
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                friendships = JsonConvert.DeserializeObject<List<FriendshipDTO>>(jsonString);
            }
            if (current_user == null)
            {
                return RedirectToAction("Login","Users");
            }
            //get friends ids
            List<int> friendsIDs = new List<int>();
            List<FriendshipDTO> myfriendships = new List<FriendshipDTO>();
            foreach (FriendshipDTO fs in friendships) {
                if (fs.user1_id == current_user.Id)
                {
                    friendsIDs.Add(fs.user2_id);
                    myfriendships.Add(fs);
                }
                else if(fs.user2_id == current_user.Id)
                {
                    friendsIDs.Add(fs.user1_id);
                    myfriendships.Add(fs);
                }
            }
            //get friends info
            List<FriendsVM> myFriends = new List<FriendsVM>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44368/api/users/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                foreach (int f_id in friendsIDs) {
                    HttpResponseMessage response = await client.GetAsync("getbyid/"+f_id);
                    string jsonString = await response.Content.ReadAsStringAsync();
                    FriendsVM friend = JsonConvert.DeserializeObject<FriendsVM>(jsonString);
                    for (int i = 0; i < friendships.Count; i++)
                    {
                        if (friendships[i].pending) {
                            if (friendships[i].user2_id == f_id)
                            {
                                friend.sentpending = true;
                                break;
                            }
                            else if (friendships[i].user1_id == f_id)
                            {
                                friend.recievedpending = true;
                                break;
                            }
                        }
                    }
                    int rat = -1;
                    var isNumberic = int.TryParse(sr, out rat);
                    if (!isNumberic)
                    {
                        sr = "";
                    }
                    bool add = false;
                    if ((sn == "" || sn == null) && (sr == "" || sr == null) && (se == "" || se == null))
                    {
                        add = true;
                    }
                    else {
                        bool okName = false;
                        bool okRating = false;
                        bool okEmail = false;
                        if (sn == "" || sn == null) {
                            okName = true;
                        }
                        if (sr == "" || sr == null)
                        {
                            okRating = true;
                        }
                        if (se == "" || se == null)
                        {
                            okEmail = true;
                        }
                        if (!okName && friend.displayName.Contains(sn)) {
                            okName = true;
                        }
                        if (!okRating && friend.rating == rat)
                        {
                            okRating = true;
                        }
                        if (!okEmail && friend.email.Contains(se))
                        {
                            okEmail = true;
                        }
                        if (okName && okEmail && okRating)
                        {
                            add = true;
                        }
                        else {
                            add = false;
                        }
                    }
                    if (add) {
                        myFriends.Add(friend);
                    }
                }
            }
            return View(myFriends);
        }

        public async System.Threading.Tasks.Task<ActionResult> AcceptRequest(int id) {
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
            //find friendship connection and accept it
            FriendshipDTO newFr = new FriendshipDTO()
            {
                user1_id = id,
                user2_id = current_user.Id
            };
            using (var client = new HttpClient()) {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(newFr);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("accept", byteContent);
            }
            return RedirectToAction("Index", "Friends");
        }

        public ActionResult AddFriend() {
            return RedirectToAction("Index","Users");
        }

        public async System.Threading.Tasks.Task<ActionResult> SendRequest(int id)
        {
            //test if user is still logged in and authorized
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            //check if user is logged in or not
            UserDTO current_user = null;
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
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
            FriendshipDTO newFriendship = new FriendshipDTO()
            {
                user1_id = current_user.Id,
                user2_id = id,
                pending = true,
                befriend_date = DateTime.Now,
                friendshipTier = "Unnoticed"
            };
            using (var client = new HttpClient()) {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                var content = JsonConvert.SerializeObject(newFriendship);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("create", byteContent);
            }
            return RedirectToAction("Index", "Users");
        }

        public async System.Threading.Tasks.Task<ActionResult> Unfriend(int id) {
            //test if user is still logged in and authorized
            HttpCookie jwtCookie = HttpContext.Request.Cookies.Get("jwt");
            //check if user is logged in or not
            UserDTO current_user = null;
            if (jwtCookie == null)
            {
                ViewData["loggedin"] = false;
                return RedirectToAction("Login", "Users");
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
            //find friendship connection
            int fr_id = -1;
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.GetAsync("all");

                string jsonString = await response.Content.ReadAsStringAsync();
                var fr_conns = JsonConvert.DeserializeObject<List<FriendshipDTO>>(jsonString);
                for (int i = 0; i < fr_conns.Count; i++)
                {
                    if ((fr_conns[i].user1_id == current_user.Id && fr_conns[i].user2_id == id) || (fr_conns[i].user2_id == current_user.Id && fr_conns[i].user1_id == id)) {
                        fr_id = fr_conns[i].Id;
                        break;
                    }
                }
            }
            if (fr_id < 0) {
                return RedirectToAction("Index", "Friends");
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie.Value);

                HttpResponseMessage response = await client.DeleteAsync(""+ fr_id);
            }

            return RedirectToAction("Index", "Friends");
        }

        public ActionResult ViewProfile(int id) {
            return RedirectToAction("ViewUserProfile", "Users", new { id = id });
        }

    }//end of class
}//end of namespace