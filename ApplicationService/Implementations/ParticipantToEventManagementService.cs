using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using Repository.Implementations;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Implementations
{
    public class ParticipantToEventManagementService
    {
        public List<ParticipantToEventDTO> GetAll()
        {
            List<ParticipantToEventDTO> participantsToEvents = new List<ParticipantToEventDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.PteRepo.Get())
                {
                    participantsToEvents.Add(new ParticipantToEventDTO
                    {
                        Id = item.Id,
                        Participant_id = item.Participant_id,
                        Event_id = item.Event_id
                    });
                }
            }

            return participantsToEvents;
        }

        public object GetById(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                ParticipantToEvent pte = unitOfWork.PteRepo.GetByID(id);
                return PTEToDto(pte);
            }
        }

        public bool Save(ParticipantToEventDTO participantToeventDto)
        {
            ParticipantToEvent pte = new ParticipantToEvent
            {
                Participant_id = participantToeventDto.Participant_id,
                Event_id = participantToeventDto.Event_id
            };

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.PteRepo.Insert(pte);
                    unitOfWork.Save();
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(ParticipantToEventDTO participantToEventDTO)
        {
            ParticipantToEvent toUpdate = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toUpdate = unitOfWork.PteRepo.GetByID(participantToEventDTO.Id);
            }
            if (toUpdate != null)
            {
                toUpdate.Participant_id = participantToEventDTO.Participant_id;
                toUpdate.Event_id = participantToEventDTO.Event_id;
            }
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.PteRepo.Update(toUpdate);
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
                    ParticipantToEvent toDel = unitOfWork.PteRepo.GetByID(id);
                    unitOfWork.PteRepo.Delete(toDel);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ParticipantToEventDTO PTEToDto(ParticipantToEvent pte)
        {
            if (pte == null)
            {
                return null;
            }
            ParticipantToEventDTO pteDto = new ParticipantToEventDTO
            {
                Id = pte.Id,
                Participant_id = pte.Participant_id,
                Event_id = pte.Event_id
            };
            return pteDto;
        }
    }
}
