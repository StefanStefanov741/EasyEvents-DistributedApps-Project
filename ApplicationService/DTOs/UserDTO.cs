using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string bio { get; set; }
        public string socialLink { get; set; }
        public float rating { get; set; }
        public DateTime? birthday { get; set; }
        public bool gender { get; set; }
        public List<int> friendsIDS { get; set; }
        public List<int> hosted_eventsIDS { get; set; }
        public List<int> visited_eventsIDS { get; set; }

        public bool Validate()
        {
            return (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password) && !String.IsNullOrEmpty(displayName) &&
                !String.IsNullOrEmpty(email));
        }
    }
}
