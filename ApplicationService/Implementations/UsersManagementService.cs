using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Implementations
{
    public class UsersManagementService
    {
        private DBContext ctx = new DBContext();
        public List<UserDTO> GetAll() {
            List<UserDTO> users = new List<UserDTO>();
            foreach (var item in ctx.Users.ToList()) { 
                users.Add(new UserDTO{
                    Id = item.Id,
                    username = item.username,
                    password = item.password,
                    displayName = item.displayName,
                    email = item.email,
                    phone_number = item.phone_number,
                    bio = item.bio,
                    socialLink = item.socialLink,
                    rating = item.rating,
                    birthday = item.birthday,
                    gender = item.gender,
                    friendsIDS = item.friendsIDS,
                    hosted_eventsIDS = item.hosted_eventsIDS,
                    visited_eventsIDS = item.visited_eventsIDS
                });
            }

            return users;
        }

        public UserDTO GetById(int id)
        {
            User user = ctx.Users.Where(u => u.Id == id).FirstOrDefault();
            return UserToDto(user);
        }

        public UserDTO GetByUsername(string username)
        {
            User user = ctx.Users.Where(u => u.username == username).FirstOrDefault();
            return UserToDto(user);
        }

        public bool Save(UserDTO userDTO) {
            User user = new User {
                username = userDTO.username,
                password = userDTO.password,
                displayName = userDTO.displayName,
                email = userDTO.email,
                phone_number = userDTO.phone_number,
                bio = userDTO.bio,
                socialLink = userDTO.socialLink,
                rating = userDTO.rating,
                birthday = userDTO.birthday,
                gender = userDTO.gender,
                friendsIDS = userDTO.friendsIDS,
                hosted_eventsIDS = userDTO.hosted_eventsIDS,
                visited_eventsIDS = userDTO.visited_eventsIDS
            };

            try
            {
                ctx.Users.Add(user);
                ctx.SaveChanges();
                return true;
            }
            catch {
                return false;
            }
        }

        public bool Update(UserDTO userDTO)
        {
            User toUpdate = ctx.Users.Find(userDTO.Id);
            if (toUpdate != null) {
                toUpdate.username = userDTO.username;
                toUpdate.password = userDTO.password;
                toUpdate.displayName = userDTO.displayName;
                toUpdate.email = userDTO.email;
                toUpdate.phone_number = userDTO.phone_number;
                toUpdate.bio = userDTO.bio;
                toUpdate.socialLink = userDTO.socialLink;
                toUpdate.rating = userDTO.rating;
                toUpdate.birthday = userDTO.birthday;
                toUpdate.gender = userDTO.gender;
                toUpdate.friendsIDS = userDTO.friendsIDS;
                toUpdate.hosted_eventsIDS = userDTO.hosted_eventsIDS;
                toUpdate.visited_eventsIDS = userDTO.visited_eventsIDS;
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
                User user = ctx.Users.Find(id);
                ctx.Users.Remove(user);
                ctx.SaveChanges();
                return true;
            }
            catch { 
                return false;
            }
        }

        public UserDTO FindUserByUsername(string username)
        {
            try
            {
                User user = ctx.Users.Where(u => u.username == username).FirstOrDefault();
                if (user != null)
                {
                    return UserToDto(user);
                }
                else {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public UserDTO FindUserByEmail(string email)
        {
            try
            {
                User user = ctx.Users.Where(u => u.email == email).FirstOrDefault();
                if (user != null)
                {
                    return UserToDto(user);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public UserDTO FindUserByDisplayName(string Dname)
        {
            try
            {
                User user = ctx.Users.Where(u => u.displayName == Dname).FirstOrDefault();
                if (user != null)
                {
                    return UserToDto(user);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public UserDTO TryLoginUser(string un, string up)
        {
            User temp_user = ctx.Users.Where(u => u.username == un && u.password == up).FirstOrDefault();
            return UserToDto(temp_user);
        }

        public UserDTO UserToDto(User user) {
            if (user == null) {
                return null;
            }
            UserDTO uDto = new UserDTO
            {
                Id = user.Id,
                username = user.username,
                password = user.password,
                displayName = user.displayName,
                email = user.email,
                phone_number = user.email,
                bio = user.bio,
                socialLink = user.socialLink,
                rating = user.rating,
                birthday = user.birthday,
                gender = user.gender,
                friendsIDS = user.friendsIDS,
                hosted_eventsIDS = user.hosted_eventsIDS,
                visited_eventsIDS = user.visited_eventsIDS
            };
            return uDto;
        }

    }
}
