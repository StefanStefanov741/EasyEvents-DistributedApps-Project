using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Implementations
{
    public class FriendshipsManagementService
    {
        public List<FriendshipDTO> GetAll()
        {
            List<FriendshipDTO> friends = new List<FriendshipDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.FriendshipRepo.Get())
                {
                    FriendshipDTO fdto = new FriendshipDTO
                    {
                        Id = item.Id,
                        user1_id = item.user1_id,
                        user2_id = item.user2_id,
                        befriend_date = item.befriend_date,
                        pending = item.pending,
                        friendshipTier = item.friendshipTier
                    };
                    if (fdto.friendshipTier == "Friends")
                    {
                        List<ParticipantToEvent> user_1events = unitOfWork.PteRepo.Get(p => p.Participant_id == fdto.user1_id).ToList();
                        List<ParticipantToEvent> user_2events = unitOfWork.PteRepo.Get(p => p.Participant_id == fdto.user2_id).ToList();
                        int common_events = 0;
                        for (int i = 0; i < user_1events.Count; i++)
                        {
                            for (int j = 0; j < user_2events.Count; j++)
                            {
                                if (user_1events[i].Id == user_2events[j].Id) {
                                    common_events++;
                                }
                            }
                        }
                        if (common_events >= 10) {
                            fdto.friendshipTier = "Best Friends";
                        }
                    }
                    if (fdto.friendshipTier == "Acquaintances" && (DateTime.Now > fdto.befriend_date.AddDays(7))) {
                        fdto.friendshipTier = "Friends";
                        Update(fdto);
                    }
                    friends.Add(fdto);
                }
            }

            return friends;
        }

        public bool AcceptFriendship(int id1, int id2) {
            List<FriendshipDTO> friendships = GetAll();
            FriendshipDTO fr = null;
            for (int i = 0; i < friendships.Count; i++)
            {
                if ((friendships[i].user1_id == id1 && friendships[i].user2_id == id2) || (friendships[i].user1_id == id2 && friendships[i].user2_id == id1)) {
                    fr = friendships[i];
                    break;
                }
            }
            if (fr == null)
            {
                return false;
            }
            else {
                fr.pending = false;
                fr.friendshipTier = "Acquaintances";
                return Update(fr);
            }
        }

        public object GetById(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Friendship friend = unitOfWork.FriendshipRepo.GetByID(id);
                return FriendshipToDTO(friend);
            }
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
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.FriendshipRepo.Insert(friend);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(FriendshipDTO friendDTO)
        {
            Friendship toUpdate = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toUpdate = unitOfWork.FriendshipRepo.GetByID(friendDTO.Id);
            }
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
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.FriendshipRepo.Update(toUpdate);
                    unitOfWork.Save();
                }
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
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    Friendship friend = unitOfWork.FriendshipRepo.GetByID(id);
                    unitOfWork.UserRepo.Delete(friend);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public FriendshipDTO FriendshipToDTO(Friendship fr)
        {
            if (fr == null)
            {
                return null;
            }
            FriendshipDTO frDto = new FriendshipDTO
            {
                Id = fr.Id,
                user1_id = fr.user1_id,
                user2_id = fr.user2_id,
                befriend_date = fr.befriend_date,
                pending = fr.pending,
                friendshipTier = fr.friendshipTier
            };
            return frDto;
        }

    }
}
