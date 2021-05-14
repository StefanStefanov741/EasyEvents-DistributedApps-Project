using ApplicationService.DTOs;
using ApplicationService.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        //user methods
        private UsersManagementService usersService = new UsersManagementService();
        public List<UserDTO> GetAllUsers()
        {
            return usersService.GetAll();
        }

        public string PostUser(UserDTO userDto)
        {
            if (usersService.Save(userDto))
            {
                return "User saved!";
            }
            else {
                return "User is not saved!";
            }
        }

        public string DeleteUser(int id)
        {
            if (usersService.Delete(id))
            {
                return "User deleted!";
            }
            else
            {
                return "User is not deleted!";
            }
        }

        public string UpdateUser(UserDTO userDto)
        {
            if (usersService.Update(userDto))
            {
                return "User updated!";
            }
            else
            {
                return "User is not updated!";
            }
        }

        public UserDTO GetUserByUsername(string username)
        {
            UserDTO u = usersService.FindUserByUsername(username);
            return u;
        }

        public UserDTO GetUserByEmail(string email)
        {
            UserDTO u = usersService.FindUserByEmail(email);
            return u;
        }

        public UserDTO GetUserByDisplayName(string Dname)
        {
            UserDTO u = usersService.FindUserByDisplayName(Dname);
            return u;
        }

        public UserDTO TryLogin(string username, string password)
        {
            UserDTO u = usersService.TryLoginUser(username,password);
            return u;
        }
        //friendships methods
        private FriendshipsManagementService friendsService = new FriendshipsManagementService();
        public List<FriendshipDTO> GetAllFriends()
        {
            return friendsService.GetAll();
        }

        public string PostFriend(FriendshipDTO friendDto)
        {
            if (friendsService.Save(friendDto))
            {
                return "Freindship saved!";
            }
            else
            {
                return "Freindship is not saved!";
            }
        }

        public string UpdateFriend(FriendshipDTO friendDto)
        {
            if (friendsService.Update(friendDto))
            {
                return "Freindship updated!";
            }
            else
            {
                return "Freindship is not updated!";
            }
        }

        public string DeleteFriend(int id)
        {
            if (friendsService.Delete(id))
            {
                return "Freindship deleted!";
            }
            else
            {
                return "Freindship is not deleted!";
            }
        }

        //events methods
        private EventsManagementService eventsServices = new EventsManagementService();
        public List<EventDTO> GetAllEvents()
        {
            return eventsServices.GetAll();
        }

        public string PostEvent(EventDTO eventDto)
        {
            if (eventsServices.Save(eventDto))
            {
                return "Event saved!";
            }
            else
            {
                return "Event is not saved!";
            }
        }

        public string UpdateEvent(EventDTO eventDto)
        {
            if (eventsServices.Update(eventDto))
            {
                return "Event updated!";
            }
            else
            {
                return "Event is not updated!";
            }
        }

        public string DeleteEvent(int id)
        {
            if (eventsServices.Delete(id))
            {
                return "Event deleted!";
            }
            else
            {
                return "Event is not deleted!";
            }
        }


    }
}
