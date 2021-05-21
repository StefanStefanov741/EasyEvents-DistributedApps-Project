using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class LikeDTO
    {
        public int Id;
        public int User_id;
        public int Event_id;
        public bool Validate()
        {
            return (User_id != 0 && Event_id != 0);
        }
    }
}
