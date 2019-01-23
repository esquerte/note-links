using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Extensions;
using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NoteLinks.Data.Test
{
    public class NotesPagingTests
    {
        private MainContext _context;
        private List<Note> _noteList;

        public NotesPagingTests()
        {
            _context = GetInMemoryDataContext();

            _noteList = new List<Note>()
            {
                new Note { Id = 1, Name = "Egwene", FromDate = DateTime.Now.AddDays(-7), ToDate = DateTime.Now.AddDays(5), Text = "Blue", CalendarId = 1 },
                new Note { Id = 2, Name = "Lanfear", FromDate = DateTime.Now.AddDays(5), ToDate = DateTime.Now.AddDays(6), Text = "", CalendarId = 1 },
                new Note { Id = 3, Name = "Nynaeve", FromDate = DateTime.Now.AddDays(-4), Text = "Yellow", CalendarId = 1 },
                new Note { Id = 4, Name = "Moiraine", FromDate = DateTime.Now.AddDays(2), ToDate = DateTime.Now.AddDays(4), Text = "Blue", CalendarId = 1 },
                new Note { Id = 5, Name = "Elayne", FromDate = DateTime.Now.AddHours(6), Text = "Green", CalendarId = 2 },
                new Note { Id = 6, Name = "Cadsuane", FromDate = DateTime.Now.AddDays(3), Text = "Green", CalendarId = 2 },
                new Note { Id = 7, Name = "Min", FromDate = DateTime.Now.AddMinutes(15), Text = "", CalendarId = 2 },
            };

            _context.Notes.AddRange(_noteList);
            _context.SaveChangesAsync();
        }

        [Fact]
        public void PagingNotesShouldNotChangeQueryIfPageInfoIsNull()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 1);
            PageInfo pageInfo = null;

            // act
            query = query.Paginate(pageInfo);
            List<Note> result = query.ToList();

            // assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void PagingNotesShouldReturnNotesOrderedByNameDescending()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 2);
            var pageInfo = new PageInfo()
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Name",
                Desc = true
            };

            // act
            query = query.Paginate(pageInfo);
            List<Note> result = query.ToList();

            // assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Min", result[0].Name);
            Assert.Equal("Elayne", result[1].Name);
            Assert.Equal("Cadsuane", result[2].Name);
        }

        [Fact]
        public void PagingNotesShouldReturnNotesOrderedByFromDate()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 1);
            var pageInfo = new PageInfo()
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "FromDate"
            };

            // act
            query = query.Paginate(pageInfo);
            List<Note> result = query.ToList();

            // assert
            Assert.Equal(4, result.Count);
            Assert.Equal("Egwene", result[0].Name);
            Assert.Equal("Nynaeve", result[1].Name);
            Assert.Equal("Moiraine", result[2].Name);
            Assert.Equal("Lanfear", result[3].Name);
        }

        [Fact]
        public void PagingNotesShouldReturnNotesFromSecondPage()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 2);
            var pageInfo = new PageInfo()
            {
                OrderBy = "FromDate",
                PageIndex = 2,
                PageSize = 2
            };

            // act
            query = query.Paginate(pageInfo);
            List<Note> result = query.ToList();

            // assert
            Assert.Single(result);
            Assert.Equal("Cadsuane", result[0].Name);
        }

        private MainContext GetInMemoryDataContext()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "NoteTestDatabase")
                .Options;
            var context = new MainContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();            
            return context;
        }

    }
}
