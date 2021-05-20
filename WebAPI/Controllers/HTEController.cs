using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System.Web.Http;
using WebAPI.Messages;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/hte")]
    public class HTEController : ApiController
    {
        private readonly HostToEventManagementService _service = null;
        public HTEController()
        {
            _service = new HostToEventManagementService();
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
        public IHttpActionResult Create(HostToEventDTO hteDto)
        {
            ResponseMessage response = new ResponseMessage();

            if (hteDto.Validate() == false)
            {
                response.Code = 400;
                response.Body = "Invalid data - Connection has not been saved";

                return Json(response);
            }
            if (_service.Save(hteDto))
            {
                response.Code = 201;
                response.Body = "Connection has been saved.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Connection has not been saved.";
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
                response.Body = "Connection has been deleted.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Connection has not been deleted.";
            }

            return Json(response);
        }
    }
}
