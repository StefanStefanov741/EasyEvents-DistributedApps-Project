using Data.Entities;

namespace ApplicationService.DTOs
{
    public class ParticipantToEventDTO
    {
        public int Id;
        public int Participant_id;
        public int Event_id;

        public bool Validate()
        {
            return (Participant_id != 0 && Event_id != 0);
        }
    }
}
