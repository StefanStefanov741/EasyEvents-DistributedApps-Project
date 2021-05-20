using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Implementations
{
    public class ParticipantToEventManagementService
    {
        private DBContext ctx = new DBContext();
        public List<ParticipantToEventDTO> GetAll()
        {
            List<ParticipantToEventDTO> participantsToEvents = new List<ParticipantToEventDTO>();
            foreach (var item in ctx.ParticipantsToEvents.ToList())
            {
                participantsToEvents.Add(new ParticipantToEventDTO
                {
                    Id = item.Id,
                    Participant_id = item.Participant_id,
                    Event_id = item.Event_id
                });
            }

            return participantsToEvents;
        }

        public object GetById(int id)
        {
            ParticipantToEvent pte = ctx.ParticipantsToEvents.Where(pe => pe.Id == id).FirstOrDefault();
            return PTEToDto(pte);
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
                ctx.ParticipantsToEvents.Add(pte);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(ParticipantToEventDTO participantToEventDTO)
        {
            ParticipantToEvent toUpdate = ctx.ParticipantsToEvents.Find(participantToEventDTO.Id);
            if (toUpdate != null)
            {
                toUpdate.Participant_id = participantToEventDTO.Participant_id;
                toUpdate.Event_id = participantToEventDTO.Event_id;
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
                ParticipantToEvent pte = ctx.ParticipantsToEvents.Find(id);
                ctx.ParticipantsToEvents.Remove(pte);
                ctx.SaveChanges();
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
