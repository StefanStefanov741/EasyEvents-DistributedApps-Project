using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationService.Implementations
{
    public class LikesManagementService
    {
        private DBContext ctx = new DBContext();
        public List<LikeDTO> GetAll()
        {
            List<LikeDTO> likes = new List<LikeDTO>();
            foreach (var item in ctx.Likes.ToList())
            {
                likes.Add(new LikeDTO
                {
                    Id = item.Id,
                    User_id = item.User_id,
                    Event_id = item.Event_id
                });
            }

            return likes;
        }

        public object GetById(int id)
        {
            Like like = ctx.Likes.Where(l => l.Id == id).FirstOrDefault();
            return LikeToDto(like);
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
                ctx.Likes.Add(like);
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
                Like like = ctx.Likes.Find(id);
                ctx.Likes.Remove(like);
                ctx.SaveChanges();
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
