using ApplicationService.DTOs;
using Data.Context;
using Data.Entities;
using Repository.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApplicationService.Implementations
{
    public class UsersManagementService
    {
        public List<UserDTO> GetAll() {
            List<UserDTO> users = new List<UserDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.UserRepo.Get())
                {
                    users.Add(new UserDTO
                    {
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
                    });
                }
            }

            return users;
        }

        public List<UserDTO> GetAll(string sn,string sr)
        {
            int rat = int.Parse(sr);
            Expression<Func<User, bool>> filter = u => (u.displayName.Contains(sn))&&(u.rating==rat);
            List<UserDTO> users = new List<UserDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.UserRepo.Get(filter))
                {
                    users.Add(new UserDTO
                    {
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
                    });
                }
            }

            return users;
        }

        public List<UserDTO> GetAllSN(string sn)
        {
            Expression<Func<User, bool>> filter = u => u.displayName.Contains(sn);
            List<UserDTO> users = new List<UserDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.UserRepo.Get(filter))
                {
                    users.Add(new UserDTO
                    {
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
                    });
                }
            }

            return users;
        }

        public List<UserDTO> GetAllSR(string sr)
        {
            int rat = int.Parse(sr);
            Expression<Func<User, bool>> filter = u => u.rating == rat;
            List<UserDTO> users = new List<UserDTO>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                foreach (var item in unitOfWork.UserRepo.Get(filter))
                {
                    users.Add(new UserDTO
                    {
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
                    });
                }
            }

            return users;
        }

        public UserDTO GetById(int id)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                User user = unitOfWork.UserRepo.GetByID(id);
                return UserToDto(user);
            }
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
            };

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.UserRepo.Insert(user);
                    unitOfWork.Save();
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public bool Update(UserDTO userDTO)
        {
            User toUpdate = null;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                toUpdate = unitOfWork.UserRepo.GetByID(userDTO.Id);
            }
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
            }
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    unitOfWork.UserRepo.Update(toUpdate);
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
                    User user = unitOfWork.UserRepo.GetByID(id);
                    unitOfWork.UserRepo.Delete(user);
                    unitOfWork.Save();
                }
                return true;
            }
            catch { 
                return false;
            }
        }

        public UserDTO GetByUsername(string username)
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    User user = unitOfWork.UserRepo.Get().Where(u => u.username == username).FirstOrDefault();
                    return UserToDto(user);
                }
            }
            catch {
                return null;
            }
        }

        public UserDTO GetByEmail(string email)
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    User user = unitOfWork.UserRepo.Get().Where(u => u.email == email).FirstOrDefault();
                    return UserToDto(user);
                }
            }
            catch
            {
                return null;
            }
        }

        public UserDTO GetByDisplayName(string Dname)
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    User user = unitOfWork.UserRepo.Get().Where(u => u.displayName == Dname).FirstOrDefault();
                    return UserToDto(user);
                }
            }
            catch
            {
                return null;
            }
        }

        public UserDTO TryLoginUser(string un, string up)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                User user = unitOfWork.UserRepo.Get().Where(u => u.username == un && u.password == up).FirstOrDefault();
                return UserToDto(user);
            }
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
                phone_number = user.phone_number,
                bio = user.bio,
                socialLink = user.socialLink,
                rating = user.rating,
                birthday = user.birthday,
                gender = user.gender,
            };
            return uDto;
        }

    }
}
