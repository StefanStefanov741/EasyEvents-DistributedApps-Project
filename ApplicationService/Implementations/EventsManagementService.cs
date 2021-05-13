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
                    ended=item.ended,
                    participantsIDS=item.participantsIDS
                });
            }

            return events;
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
                ended = eventDTO.ended,
                participantsIDS = eventDTO.participantsIDS
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
                toUpdate.ended = evenyDTO.ended;
                toUpdate.participantsIDS = evenyDTO.participantsIDS;
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
    }
}
