using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using Repository.Implementations;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Implementations
{
    public class LikesManagementService
    {
        public List<LikeDTO> GetAll()
        {
            List<LikeDTO> likes = new List<LikeDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.LikeRepo.Get())
                {
                    likes.Add(new LikeDTO
                    {
                        Id = item.Id,
                        User_id = item.User_id,
                        Event_id = item.Event_id
                    });
                }
            }

            return likes;
        }

        public object GetById(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Like like = unitOfWork.LikeRepo.GetByID(id);
                return LikeToDto(like);
            }
        }

        public bool Save(LikeDTO likeDTO)
        {
            Like like = new Like
            {
                User_id = likeDTO.User_id,
                Event_id = likeDTO.Event_id
            };

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.LikeRepo.Insert(like);
                    unitOfWork.Save();
                };
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
                    Like toDel = unitOfWork.LikeRepo.GetByID(id);
                    unitOfWork.LikeRepo.Delete(toDel);
                    unitOfWork.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public LikeDTO LikeToDto(Like like)
        {
            if (like == null)
            {
                return null;
            }
            LikeDTO likeDto = new LikeDTO
            {
                Id = like.Id,
                User_id = like.User_id,
                Event_id = like.Event_id
            };
            return likeDto;
        }
    }
}
