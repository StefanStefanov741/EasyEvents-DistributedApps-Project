using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public int host_id { get; set; }
        public int likes { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime begins { get; set; }
        public bool ended { get; set; }
        public List<int> participantsIDS { get; set; }
    }
}
