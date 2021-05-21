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

        [Authorize]
        [HttpPost]
        [Route("updateevent")]
        public IHttpActionResult UpdateEventInfo(EventDTO event_info)
        {
            _service.Update(event_info);
            ResponseMessage rm = new ResponseMessage();
            rm.Code = 200;
            rm.Body = "Update complete.";
            return Json(rm);
        }

        [Authorize]
        [HttpPost]
        [Route("like")]
        public IHttpActionResult Like(LikeDTO like)
        {
            ResponseMessage response = new ResponseMessage();
            if (like.Validate() == false) {
                response.Code = 400;
                response.Body = "Event has not been liked.";
            }
            if (_service.Like(like.Event_id))
            {
                response.Code = 201;
                response.Body = "Event has been liked.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Event has not been liked.";
            }

            return Json(response);
        }

        [Authorize]
        [HttpPost]
        [Route("dislike")]
        public IHttpActionResult Dislike(EventDTO ev)
        {
            ResponseMessage response = new ResponseMessage();
            if (ev.Id == 0) {
                response.Code = 400;
                response.Body = "Event has not been disliked.";
                return Json(response);
            }
            if (_service.Dislike(ev.Id))
            {
                response.Code = 201;
                response.Body = "Event has been disliked.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Event has not been disliked.";
            }

            return Json(response);
        }

        [Authorize]
        [HttpPost]
        [Route("joinevent")]
        public IHttpActionResult Join(ParticipantToEventDTO pte)
        {
            ResponseMessage response = new ResponseMessage();
            if (pte.Validate() == false)
            {
                response.Code = 400;
                response.Body = "Event has not been liked.";
            }
            if (_service.Join(pte.Event_id))
            {
                response.Code = 201;
                response.Body = "Event has been liked.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Event has not been liked.";
            }

            return Json(response);
        }

        [Authorize]
        [HttpPost]
        [Route("leaveevent")]
        public IHttpActionResult Leave(EventDTO ev)
        {
            ResponseMessage response = new ResponseMessage();

            if (_service.Leave(ev.Id))
            {
                response.Code = 201;
                response.Body = "Event has been liked.";
            }
            else
            {
                response.Code = 200;
                response.Body = "Event has not been liked.";
            }

            return Json(response);
        }

    }
}
