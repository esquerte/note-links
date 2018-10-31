using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Entities;
using System;

namespace NoteLinks.Data.Context
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calendar>().HasData(
            new Calendar[]
            {
                new Calendar { Id=1, Code="asdf", Name="Дни рождения родственников" },
                new Calendar { Id=2, Code="qwer", Name="Отпуска сотрудников" },
                new Calendar { Id=3, Code="zxcv", Name="Поездка в Испанию" },
            });

            modelBuilder.Entity<Note>().HasData(
            new Note[]
            {
                new Note { Id=1, CalendarId=1, Name="Артём", FromDate=DateTime.Parse("18.06.2019"), Text="Хочет Lego" },
                new Note { Id=2, CalendarId=1, Name="Марина", FromDate=DateTime.Parse("15.12.2018"), Text="Пригласить Ивановых" },
                new Note { Id=3, CalendarId=1, Name="Оксана Ивановна", FromDate=DateTime.Parse("02.03.2019"), Text="Уехать в командировку" },
                new Note { Id=4, CalendarId=2, Name="Серёга", FromDate=DateTime.Parse("07.03.2019"), ToDate=DateTime.Parse("24.03.2019"), Text="Тайланд" },
                new Note { Id=5, CalendarId=2, Name="Рустем", FromDate=DateTime.Parse("08.03.2019"), ToDate=DateTime.Parse("19.03.2019"), Text="Хочет перенести на апрель" },
                new Note { Id=6, CalendarId=3, Name="Самолет Сургут-Москва", FromDate=new DateTime(2019,5,29,17,40,0), ToDate=new DateTime(2019,5,29,18,30,0), Text="Взять книгу" },
                new Note { Id=7, CalendarId=3, Name="Хостел в Москве", FromDate=new DateTime(2019,5,30), ToDate=new DateTime(2019,6,3), Text="Заезд после 15:00" },
                new Note { Id=8, CalendarId=3, Name="Самолет Москва-Барселона", FromDate=new DateTime(2019,6,3,9,20,0), ToDate=new DateTime(2019,6,3,11,5,0) }
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
