using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using WebAPI.Messages;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly UsersManagementService _service = null;

        public UsersController() {
            _service = new UsersManagementService();
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAllUsers() {
            return Json(_service.GetAll());
        }

        [Authorize]
        [HttpGet]
        [Route("getbyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            return Json(_service.GetById(id));
        }

        [Authorize]
        [HttpPost]
        [Route("getbyusername")]
        public IHttpActionResult GetByUsername(UserDTO username)
        {
            return Json(_service.GetByUsername(username.username));
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(UserDTO userDTO)
        {
            ResponseMessage response = new ResponseMessage();

            if (userDTO.Validate() == false)
            {
                response.Code = 400;
                response.Body = "Invalid data - User has not been saved";

                return Json(response);
            }
            if (_service.Save(userDTO))
            {
                response.Code = 201;
                response.Body = "User has been saved.";
            }
            else
            {
                response.Code = 200;
                response.Body = "User has not been saved.";
            }

            return Json(response);
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            ResponseMessage response = new ResponseMessage();

            if (_service.Delete(id))
            {
                response.Code = 201;
                response.Body = "User has been deleted.";
            }
            else
            {
                response.Code = 200;
                response.Body = "User has not been deleted.";
            }

            return Json(response);
        }

        [HttpPost]
        [Route("findexistinguser")]
        public IHttpActionResult FindExistingUser(UserDTO userDTO)
        {
            ResponseMessage response = new ResponseMessage();

            UserDTO u1 = _service.FindUserByUsername(userDTO.username);
            UserDTO u2 = _service.FindUserByDisplayName(userDTO.displayName);
            UserDTO u3 = _service.FindUserByEmail(userDTO.email);
            response.Code = 404;

            if (u2 != null)
            {
                response.Code = 200;
                response.Body = "Displayname exists!";
            }
            if (u1 != null) {
                response.Code = 200;
                response.Body+= "Username exists!";
            }
            if (u3 != null)
            {
                response.Code = 200;
                response.Body+= "Email exists!";
            }
            if (u1 == null && u2 == null && u3 == null) {
                response.Body = "User not found!";
            }

            return Json(response);
        }

        [HttpPost]
        [Route("trylogin")]
        public IHttpActionResult TryLogin(UserDTO user_info)
        {
            ResponseMessage response = new ResponseMessage();
            UserDTO user = _service.TryLoginUser(user_info.username, user_info.password);
            if (user != null)
            {
                response.Code = 200;
                response.Body = "User credentials are correct.";
                return Json(response);
            }
            else {
                response.Code = 403;
                response.Body = "Username or password are incorrect!";
                return Json(response);
            }
        }

        [HttpPost]
        [Route("auth")]
        public IHttpActionResult Authenticate(UserDTO u)
        {
            var userResponse = new UserResponse { };
            UserRequest userRequest = new UserRequest { };
            userRequest.Username = u.username;
            userRequest.Password = u.password;

            string token = createToken(userRequest);

            ResponseMessage token_message = new ResponseMessage();
            token_message.Code = 201;
            token_message.Body = token;
            return Json(token_message);
        }

        private string createToken(UserRequest user)
        {
            DateTime issuedAt = DateTime.UtcNow;
            DateTime expires = DateTime.UtcNow.AddHours(2);

            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Name, user.Password)
            });

            const string sec = "z401b09eab3c013d4ca54922bb802bec8fd5wefgewgGHA318192b0a75f201dwegwGWGW8b3727429090fb33759fefaeH5THE1abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var now = DateTime.UtcNow;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

            var token =
                (JwtSecurityToken)
                    tokenHandler.CreateJwtSecurityToken(
                        issuer: "https://localhost:44368/",
                        audience: "https://localhost:44368/",
                        subject: claimsIdentity,
                        notBefore: issuedAt,
                        expires: expires,
                        signingCredentials: signingCredentials
                        );
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        [Authorize]
        [HttpPost]
        [Route("userfromtoken")]
        public IHttpActionResult UserFromToken(HttpCookie c)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(c.Value);
            var tokenS = jsonToken as JwtSecurityToken;
            var claims = tokenS.Claims.First(claim => claim.Type == "unique_name").Value;
            ResponseMessage rm = new ResponseMessage();
            rm.Code = 201;
            rm.Body = claims.ToString();
            return Json(rm);
        }

        [Authorize]
        [HttpPost]
        [Route("updateuser")]
        public IHttpActionResult UpdateUserInfo(UserDTO user_info)
        {
            UserDTO user = _service.GetById(user_info.Id);
            user.username = user_info.username;
            user.password = user_info.password;
            user.email = user_info.email;
            user.displayName = user_info.displayName;
            user.phone_number = user_info.phone_number;
            user.bio = user_info.bio;
            user.gender = user_info.gender;
            user.socialLink = user_info.socialLink;
            _service.Update(user);
            ResponseMessage rm = new ResponseMessage();
            rm.Code = 200;
            rm.Body = "Update complete.";
            return Json(rm);
        }

    }
}
