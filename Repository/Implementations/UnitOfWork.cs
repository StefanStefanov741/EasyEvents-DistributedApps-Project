using Data.Context;
using Data.Entities;
using System;

namespace Repository.Implementations
{
    public class UnitOfWork : IDisposable
    {
        private DBContext context = new DBContext();
        private GenericRepository<Event> eventRepo;
        private GenericRepository<User> userRepo;
        private GenericRepository<Friendship> friendshipRepo;
        private GenericRepository<HostToEvent> hteRepo;
        private GenericRepository<ParticipantToEvent> pteRepo;
        private GenericRepository<Like> likeRepo;

        public GenericRepository<Event> EventRepo
        {
            get
            {

                if (this.eventRepo == null)
                {
                    this.eventRepo = new GenericRepository<Event>(context);
                }
                return eventRepo;
            }
        }

        public GenericRepository<User> UserRepo
        {
            get
            {

                if (this.userRepo == null)
                {
                    this.userRepo = new GenericRepository<User>(context);
                }
                return userRepo;
            }
        }

        public GenericRepository<Friendship> FriendshipRepo
        {
            get
            {

                if (this.friendshipRepo == null)
                {
                    this.friendshipRepo = new GenericRepository<Friendship>(context);
                }
                return friendshipRepo;
            }
        }

        public GenericRepository<HostToEvent> HteRepo
        {
            get
            {

                if (this.hteRepo == null)
                {
                    this.hteRepo = new GenericRepository<HostToEvent>(context);
                }
                return hteRepo;
            }
        }

        public GenericRepository<ParticipantToEvent> PteRepo
        {
            get
            {

                if (this.pteRepo == null)
                {
                    this.pteRepo = new GenericRepository<ParticipantToEvent>(context);
                }
                return pteRepo;
            }
        }

        public GenericRepository<Like> LikeRepo
        {
            get
            {

                if (this.likeRepo == null)
                {
                    this.likeRepo = new GenericRepository<Like>(context);
                }
                return likeRepo;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
