﻿using Data.Entities;
using System.Data.Entity;

namespace Data.Context
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friends { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}