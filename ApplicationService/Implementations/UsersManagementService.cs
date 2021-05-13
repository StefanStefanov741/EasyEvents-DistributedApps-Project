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
    }
}
