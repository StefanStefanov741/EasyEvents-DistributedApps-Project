using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Messages;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/friends")]
    public class FriendsController : ApiController
    {
        private readonly FriendshipsManagementService _service = null;
        public FriendsController()
        {
            _service = new FriendshipsManagementService();
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            return Json(_service.GetAll());
        }

        [HttpGet]
        [Route("all/{tier}")]
        public IHttpActionResult GetAll(string tier)
        {
            return Json(_service.GetAll(tier));
        }

        [HttpGet]
        [Route("getbyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            return Json(_service.GetById(id));
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(FriendshipDTO frndDto)
        {
            ResponseMessage response = new ResponseMessage();

            if (frndDto.Validate() == false)
            {
                response.Code = 400;
                response.Body = "Invalid data - Friendship has not been saved";

                return Json(response);
            }
            if (_service.Save(frndDto))
            {
                response.Code = 201;
                response.Body = "Friendship has been saved.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Friendship has not been saved.";
            }

            return Json(response);
        }

        [Authorize]
        [HttpPost]
        [Route("accept")]
        public IHttpActionResult AcceptFriend(FriendshipDTO fdto) {
            ResponseMessage response = new ResponseMessage();
            if (_service.AcceptFriendship(fdto.user1_id,fdto.user2_id))
            {
                response.Code = 201;
                response.Body = "Friendship has been saved.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Friendship has not been saved.";
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
                response.Body = "Friendship has been deleted.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Friendship has not been deleted.";
            }

            return Json(response);
        }
    }
}
