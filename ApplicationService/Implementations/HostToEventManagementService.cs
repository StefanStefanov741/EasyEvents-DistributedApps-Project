using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Implementations
{
    public class HostToEventManagementService
    {
        private DBContext ctx = new DBContext();
        public List<HostToEventDTO> GetAll()
        {
            List<HostToEventDTO> hostToEvents = new List<HostToEventDTO>();
            foreach (var item in ctx.HostToEvents.ToList())
            {
                hostToEvents.Add(new HostToEventDTO
                {
                    Id = item.Id,
                    Host_id = item.Host_id,
                    Event_id = item.Event_id
                });
            }

            return hostToEvents;
        }

        public object GetById(int id)
        {
            HostToEvent hte = ctx.HostToEvents.Where(he => he.Id == id).FirstOrDefault();
            return HTEToDto(hte);
        }

        public bool Save(HostToEventDTO hostToEventDTO)
        {
            HostToEvent hte = new HostToEvent
            {
                Host_id = hostToEventDTO.Host_id,
                Event_id = hostToEventDTO.Event_id
            };

            try
            {
                ctx.HostToEvents.Add(hte);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(HostToEventDTO hostToEventDTO)
        {
            HostToEvent toUpdate = ctx.HostToEvents.Find(hostToEventDTO.Id);
            if (toUpdate != null)
            {
                toUpdate.Host_id = hostToEventDTO.Host_id;
                toUpdate.Event_id = hostToEventDTO.Event_id;
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
                HostToEvent hte = ctx.HostToEvents.Find(id);
                ctx.HostToEvents.Remove(hte);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public HostToEventDTO HTEToDto(HostToEvent hte)
        {
            if (hte == null)
            {
                return null;
            }
            HostToEventDTO hteDto = new HostToEventDTO
            {
                Id = hte.Id,
                Host_id = hte.Host_id,
                Event_id = hte.Event_id
            };
            return hteDto;
        }
    }
}
