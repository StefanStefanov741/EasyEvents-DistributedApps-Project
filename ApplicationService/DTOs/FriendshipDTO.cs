using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class FriendshipDTO
    {
        public int Id { get; set; }
        public int user1_id { get; set; }
        public int user2_id { get; set; }
        public DateTime befriend_date { get; set; }
        public bool pending { get; set; }
        public string friendshipTier { get; set; }

        public bool Validate()
        {
            return (Id!=0 && user1_id!=0 && user2_id != 0);
        }
    }
}
