using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;

namespace NoteLinks.Data.Context
{
    public class MainContext : IdentityDbContext<User>
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCalendar>()
                .HasKey(uc => new { uc.UserId, uc.CalendarId });

            modelBuilder.Entity<UserCalendar>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCalendars)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCalendar>()
                .HasOne(uc => uc.Calendar)
                .WithMany(c => c.UserCalendars)
                .HasForeignKey(uc => uc.CalendarId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
