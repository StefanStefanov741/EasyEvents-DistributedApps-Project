using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
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

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAllUsers() {
            return Json(_service.GetAll());
        }

        [HttpGet]
        [Route("getbyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            return Json(_service.GetById(id));
        }

        [HttpGet]
        [Route("getbyusername/{username}")]
        public IHttpActionResult GetByUsername(string username)
        {
            return Json(_service.GetByUsername(username));
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

        [HttpGet]
        [Route("findbyusername/{username}")]
        public IHttpActionResult FindByUsername(string username)
        {
            string real_username = username.Replace("4242424242424242", ".");
            ResponseMessage response = new ResponseMessage();
            if (_service.FindUserByUsername(real_username) != null)
            {
                response.Code = 200;
                response.Body = "User exists.";
            }
            else {
                response.Code = 404;
                response.Body = "User doesn't exist.";
            }
            return Json(response);
        }

        [HttpGet]
        [Route("findbyemail/{email}")]
        public IHttpActionResult FindByEmail(string email)
        {
            string real_email = email.Replace("4242424242424242", ".");
            ResponseMessage response = new ResponseMessage();
            if (_service.FindUserByEmail(real_email) != null)
            {
                response.Code = 200;
                response.Body = "User exists.";
            }
            else
            {
                response.Code = 404;
                response.Body = "User doesn't exist.";
            }
            return Json(response);
        }

        [HttpGet]
        [Route("findbydispn/{dispN}")]
        public IHttpActionResult FindByDisplayName(string dispN)
        {
            string real_dispN = dispN.Replace("4242424242424242", ".");
            ResponseMessage response = new ResponseMessage();
            if (_service.FindUserByDisplayName(real_dispN) != null)
            {
                response.Code = 200;
                response.Body = "User exists.";
            }
            else
            {
                response.Code = 404;
                response.Body = "User doesn't exist.";
            }
            return Json(response);
        }

        [HttpGet]
        [Route("trylogin/{username}/{password}")]
        public IHttpActionResult TryLogin(string username,string password)
        {
            ResponseMessage response = new ResponseMessage();
            UserDTO user = _service.TryLoginUser(username, password);
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
        [Route("auth/{username}/{password}")]
        public IHttpActionResult Authenticate(string username,string password)
        {
            var userResponse = new UserResponse { };
            UserRequest userRequest = new UserRequest { };
            userRequest.Username = username;
            userRequest.Password = password;

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

    }
}
