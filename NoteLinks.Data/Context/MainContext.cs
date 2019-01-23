using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Entities;
using System;

namespace NoteLinks.Data.Context
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
