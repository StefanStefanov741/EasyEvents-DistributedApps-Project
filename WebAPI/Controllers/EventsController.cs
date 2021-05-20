using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System.Web.Http;
using WebAPI.Messages;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/events")]
    public class EventsController : ApiController
    {
        private readonly EventsManagementService _service = null;

        public EventsController() {
            _service = new EventsManagementService();
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll() {
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
        [Route("getid")]
        public IHttpActionResult GetID(EventDTO eDTO)
        {
            return Json(_service.GiveID(eDTO));
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(EventDTO eventDto)
        {
            ResponseMessage response = new ResponseMessage();

            if (eventDto.Validate() == false)
            {
                response.Code = 400;
                response.Body = "Invalid data - Event has not been saved";

                return Json(response);
            }
            if (_service.Save(eventDto))
            {
                response.Code = 201;
                response.Body = "Event has been saved.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Event has not been saved.";
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
                response.Body = "Event has been deleted.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Event has not been deleted.";
            }

            return Json(response);
        }

    }
}
