using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System.Web.Http;
using WebAPI.Messages;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/pte")]
    public class PTEController : ApiController
    {
        private readonly ParticipantToEventManagementService _service = null;
        public PTEController()
        {
            _service = new ParticipantToEventManagementService();
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
        public IHttpActionResult Create(ParticipantToEventDTO pteDto)
        {
            ResponseMessage response = new ResponseMessage();

            if (pteDto.Validate() == false)
            {
                response.Code = 400;
                response.Body = "Invalid data - Connection has not been saved";

                return Json(response);
            }
            if (_service.Save(pteDto))
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
