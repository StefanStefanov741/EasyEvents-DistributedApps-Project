using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using Repository.Implementations;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Implementations
{
    public class HostToEventManagementService
    {
        public List<HostToEventDTO> GetAll()
        {
            List<HostToEventDTO> hostToEvents = new List<HostToEventDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.HteRepo.Get())
                {
                    hostToEvents.Add(new HostToEventDTO
                    {
                        Id = item.Id,
                        Host_id = item.Host_id,
                        Event_id = item.Event_id
                    });
                }
            }

            return hostToEvents;
        }

        public object GetById(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                HostToEvent hte = unitOfWork.HteRepo.GetByID(id);
                return HTEToDto(hte);
            }
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
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.HteRepo.Insert(hte);
                    unitOfWork.Save();
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(HostToEventDTO hostToEventDTO)
        {
            HostToEvent toUpdate = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toUpdate = unitOfWork.HteRepo.GetByID(hostToEventDTO.Id);
            }
            if (toUpdate != null)
            {
                toUpdate.Host_id = hostToEventDTO.Host_id;
                toUpdate.Event_id = hostToEventDTO.Event_id;
            }
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.HteRepo.Update(toUpdate);
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
                    HostToEvent toDel = unitOfWork.HteRepo.GetByID(id);
                    unitOfWork.HteRepo.Delete(toDel);
                    unitOfWork.Save();
                }
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
