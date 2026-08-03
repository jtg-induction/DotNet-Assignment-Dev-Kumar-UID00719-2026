using dotNetAssignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext() : base("AppDbContext")
        {
        }

        public DbSet<Users> Users { get; set; }

        public DbSet<UserAddresses> UserAddresses { get; set; }

        public DbSet<Restaurants> Restaurants { get; set; }

        public DbSet<RestaurantOwners> RestaurantOwners { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orders>()
                .HasRequired(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Menu)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}