using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class EventsManagementService
    {
        private DBContext ctx = new DBContext();
        public List<EventDTO> GetAll()
        {
            List<EventDTO> events = new List<EventDTO>();
            foreach (var item in ctx.Events.ToList())
            {
                events.Add(new EventDTO
                {
                    Id = item.Id,
                    title=item.title,
                    description=item.description,
                    location=item.location,
                    host_id=item.host_id,
                    likes=item.likes,
                    createdOn=item.createdOn,
                    begins=item.begins,
                    ends = item.ends,
                    ended=item.ended,
                    participants=item.participants
                });
            }

            return events;
        }

        public object GetById(int id)
        {
            Event ev = ctx.Events.Where(e => e.Id == id).FirstOrDefault();
            return EventToDto(ev);
        }

        public int GiveID(EventDTO ev) {
            int id = 0;
            foreach (var item in ctx.Events.ToList())
            {
                if (item.host_id == ev.host_id && item.title == ev.title && item.location == ev.location) {
                    id = item.Id;
                }
            }
            return id;
        }

        public bool Save(EventDTO eventDTO)
        {
            Event ev = new Event
            {
                title = eventDTO.title,
                description = eventDTO.description,
                location = eventDTO.location,
                host_id = eventDTO.host_id,
                likes = eventDTO.likes,
                createdOn = eventDTO.createdOn,
                begins = eventDTO.begins,
                ends = eventDTO.ends,
                ended = eventDTO.ended,
                participants = eventDTO.participants
            };

            try
            {
                ctx.Events.Add(ev);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(EventDTO evenyDTO)
        {
            Event toUpdate = ctx.Events.Find(evenyDTO.Id);
            if (toUpdate != null)
            {
                toUpdate.title = evenyDTO.title;
                toUpdate.description = evenyDTO.description;
                toUpdate.location = evenyDTO.location;
                toUpdate.host_id = evenyDTO.host_id;
                toUpdate.likes = evenyDTO.likes;
                toUpdate.createdOn = evenyDTO.createdOn;
                toUpdate.begins = evenyDTO.begins;
                toUpdate.ends = evenyDTO.ends;
                toUpdate.ended = evenyDTO.ended;
                toUpdate.participants = evenyDTO.participants;
            }
            try
            {
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                Event ev = ctx.Events.Find(id);
                ctx.Events.Remove(ev);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public EventDTO EventToDto(Event ev)
        {
            if (ev == null)
            {
                return null;
            }
            EventDTO eDto = new EventDTO
            {
                Id = ev.Id,
                title = ev.title,
                description = ev.description,
                location = ev.location,
                host_id = ev.host_id,
                likes = ev.likes,
                createdOn = ev.createdOn,
                begins = ev.begins,
                ends = ev.ends,
                ended = ev.ended,
                participants = ev.participants
            };
            return eDto;
        }

    }
}
