using DatingProgram.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace DatingProgram.DB
{
    public class MyDbContext : DbContext
    {
        private protected string connectionString = @"Data Source=DESKTOP-N2FEHOR\MSSQLSERVER01;Initial Catalog=DatingDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseSqlServer(connectionString));
        }

        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientPhoto> ClientPhotos { get; set; }
        public DbSet<DatingForm> DatingForms { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Pair> Pairs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasData
                (
                    new Role { Id = 1, Name = "Администратор"},
                    new Role { Id = 2, Name = "Клиент"}
                );
            modelBuilder.Entity<Likes>()
                .HasOne(l => l.FromUser)
                .WithMany(u=>u.FromClient)
                .HasForeignKey(l => l.FromUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.ToUser)
                .WithMany(u => u.ToClient)
                .HasForeignKey(l => l.ToUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Pair>()
                .HasOne(p => p.Man)
                .WithMany(c => c.ManPairs)
                .HasForeignKey(p => p.ManId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pair>()
                .HasOne(p => p.Women)
                .WithMany(c => c.WomanPairs)
                .HasForeignKey(p => p.GirlId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
