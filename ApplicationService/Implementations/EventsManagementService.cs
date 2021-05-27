using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class EventsManagementService
    {
        public List<EventDTO> GetAll()
        {
            List<EventDTO> events = new List<EventDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.EventRepo.Get())
                {
                    events.Add(new EventDTO
                    {
                        Id = item.Id,
                        title = item.title,
                        description = item.description,
                        location = item.location,
                        host_id = item.host_id,
                        likes = item.likes,
                        createdOn = item.createdOn,
                        begins = item.begins,
                        ends = item.ends,
                        ended = item.ended,
                        participants = item.participants
                    });
                }
            }

            return events;
        }

        public List<EventDTO> GetAllST(string st)
        {
            Expression<Func<Event, bool>> filter = ev => ev.title.Contains(st);
            List<EventDTO> events = new List<EventDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.EventRepo.Get(filter))
                {
                    events.Add(new EventDTO
                    {
                        Id = item.Id,
                        title = item.title,
                        description = item.description,
                        location = item.location,
                        host_id = item.host_id,
                        likes = item.likes,
                        createdOn = item.createdOn,
                        begins = item.begins,
                        ends = item.ends,
                        ended = item.ended,
                        participants = item.participants
                    });
                }
            }

            return events;
        }

        public List<EventDTO> GetAllSL(string sl)
        {
            Expression<Func<Event, bool>> filter = ev => ev.location.Contains(sl);
            List<EventDTO> events = new List<EventDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.EventRepo.Get(filter))
                {
                    events.Add(new EventDTO
                    {
                        Id = item.Id,
                        title = item.title,
                        description = item.description,
                        location = item.location,
                        host_id = item.host_id,
                        likes = item.likes,
                        createdOn = item.createdOn,
                        begins = item.begins,
                        ends = item.ends,
                        ended = item.ended,
                        participants = item.participants
                    });
                }
            }

            return events;
        }

        public List<EventDTO> GetAll(string st,string sl)
        {
            Expression<Func<Event, bool>> filter = ev => (ev.location.Contains(sl)) &&(ev.title.Contains(st));
            List<EventDTO> events = new List<EventDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.EventRepo.Get(filter))
                {
                    events.Add(new EventDTO
                    {
                        Id = item.Id,
                        title = item.title,
                        description = item.description,
                        location = item.location,
                        host_id = item.host_id,
                        likes = item.likes,
                        createdOn = item.createdOn,
                        begins = item.begins,
                        ends = item.ends,
                        ended = item.ended,
                        participants = item.participants
                    });
                }
            }

            return events;
        }

        public object GetById(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Event ev = unitOfWork.EventRepo.GetByID(id);
                return EventToDto(ev);
            }
        }

        public int GiveID(EventDTO ev) {
            int id = 0;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                var events = unitOfWork.EventRepo.Get();
                foreach (var item in events)
                {
                    if (item.host_id == ev.host_id && item.title == ev.title && item.location == ev.location)
                    {
                        id = item.Id;
                    }
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
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EventRepo.Insert(ev);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(EventDTO evenyDTO)
        {
            Event toUpdate = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toUpdate = unitOfWork.EventRepo.GetByID(evenyDTO.Id);
            }
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
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EventRepo.Update(toUpdate);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Like(int id)
        {
            Event toLike = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toLike = unitOfWork.EventRepo.GetByID(id);
            }
            if (toLike != null)
            {
                toLike.likes++;
            }
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EventRepo.Update(toLike);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Dislike(int id)
        {
            Event toDislike = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toDislike = unitOfWork.EventRepo.GetByID(id);
            }
            if (toDislike != null)
            {
                toDislike.likes--;
            }
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EventRepo.Update(toDislike);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Join(int id)
        {
            Event toJoin = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toJoin = unitOfWork.EventRepo.GetByID(id);
            }
            if (toJoin != null)
            {
                toJoin.participants++;
            }
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EventRepo.Update(toJoin);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Leave(int id)
        {
            Event toLeave = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toLeave = unitOfWork.EventRepo.GetByID(id);
            }
            if (toLeave != null)
            {
                toLeave.participants--;
            }
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EventRepo.Update(toLeave);
                    unitOfWork.Save();
                }
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
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    Event ev = unitOfWork.EventRepo.GetByID(id);
                    unitOfWork.EventRepo.Delete(ev);
                    unitOfWork.Save();
                }
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
