using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class FriendshipsManagementService
    {
        private DBContext ctx = new DBContext();
        public List<FriendshipDTO> GetAll()
        {
            List<FriendshipDTO> friends = new List<FriendshipDTO>();
            foreach (var item in ctx.Friends.ToList())
            {
                friends.Add(new FriendshipDTO
                {
                    Id = item.Id,
                    user1_id = item.user1_id,
                    user2_id = item.user2_id,
                    befriend_date=item.befriend_date,
                    pending = item.pending,
                    friendshipTier = item.friendshipTier
                });
            }

            return friends;
        }

        public bool Save(FriendshipDTO friendsDTO)
        {
            Friendship friend = new Friendship
            {
                user1_id = friendsDTO.user1_id,
                user2_id = friendsDTO.user2_id,
                befriend_date = friendsDTO.befriend_date,
                pending = friendsDTO.pending,
                friendshipTier = friendsDTO.friendshipTier
            };

            try
            {
                ctx.Friends.Add(friend);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(FriendshipDTO friendDTO)
        {
            Friendship toUpdate = ctx.Friends.Find(friendDTO.Id);
            if (toUpdate != null)
            {
                toUpdate.user1_id = friendDTO.user1_id;
                toUpdate.user2_id = friendDTO.user2_id;
                toUpdate.befriend_date = friendDTO.befriend_date;
                toUpdate.pending = friendDTO.pending;
                toUpdate.friendshipTier = friendDTO.friendshipTier;
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
                Friendship friend = ctx.Friends.Find(id);
                ctx.Friends.Remove(friend);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
