using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Messages;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/likes")]
    public class LikesController : ApiController
    {
        private readonly LikesManagementService _service = null;
        public LikesController()
        {
            _service = new LikesManagementService();
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            return Json(_service.GetAll());
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
        public IHttpActionResult Create(LikeDTO likeDto)
        {
            ResponseMessage response = new ResponseMessage();

            if (likeDto.Validate() == false)
            {
                response.Code = 400;
                response.Body = "Invalid data - Like has not been saved";

                return Json(response);
            }
            if (_service.Save(likeDto))
            {
                response.Code = 201;
                response.Body = "Like has been saved.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Like has not been saved.";
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
                response.Body = "Like has been deleted.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Like has not been deleted.";
            }

            return Json(response);
        }
    }
}