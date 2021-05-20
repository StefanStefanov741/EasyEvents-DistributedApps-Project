using Data.Entities;

namespace ApplicationService.DTOs
{
    public class HostToEventDTO
    {
        public int Id;
        public int Host_id;
        public int Event_id;
        public bool Validate()
        {
            return (Host_id!=0 && Event_id!=0);
        }
    }
}
