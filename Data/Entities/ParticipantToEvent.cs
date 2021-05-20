using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class ParticipantToEvent : BaseEntity
    {
        [Required]
        public int Participant_id { get; set; }
        [Required]
        public int Event_id { get; set; }
    }
}
