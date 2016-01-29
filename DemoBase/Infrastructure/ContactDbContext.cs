﻿using System.Data.Entity;
using DemoBase.Table;

namespace DemoBase.Infrastructure
{
    public class ContactDbContext : BaseContext<ContactDbContext>
    {
        public ContactDbContext()
            : base("name=RemoveBackgroundEntities")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = false;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CornerstoneLists>();
        }
    }
}