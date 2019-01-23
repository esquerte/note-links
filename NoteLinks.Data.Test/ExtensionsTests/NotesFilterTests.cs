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
    public class NotesFilterTests
    {
        private MainContext _context;
        private List<Note> _noteList;

        public NotesFilterTests()
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
        public void FilterNotesShouldNotChangeQueryIfFiltersIsNull()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 2);
            Filter[] filters = null;

            // act
            query = query.Filter(filters);
            var result = query.ToList();

            // assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void FilterNotesShouldReturnNotesFilteredByNameEqual()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 2);
            var filters = new Filter[] {
                new Filter() { Field = "Name", Operator = "eq", Value = "Cadsuane" }
            };

            // act
            query = query.Filter(filters);
            var result = query.ToList();

            // assert
            Assert.Single(result);
            Assert.Equal("Cadsuane", result[0].Name);
        }

        [Fact]
        public void FilterNotesShouldReturnNotesFilteredByNameContains()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 2);
            var filters = new Filter[] {
                new Filter() { Field = "Name", Operator = "ct", Value = "ne" }
            };

            // act
            query = query.Filter(filters);
            var result = query.OrderBy(x => x.Name).ToList();

            // assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Cadsuane", result[0].Name);
            Assert.Equal("Elayne", result[1].Name);
        }

        [Fact]
        public void FilterNotesShouldReturnNotesFilteredByTextEqual()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 1);
            var filters = new Filter[] {
                new Filter() { Field = "Text", Operator = "eq", Value = "Yellow" }
            };

            // act
            query = query.Filter(filters);
            var result = query.ToList();

            // assert
            Assert.Single(result);
            Assert.Equal("Yellow", result[0].Text);
        }

        [Fact]
        public void FilterNotesShouldReturnNotesFilteredByTextContains()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 1);
            var filters = new Filter[] {
                new Filter() { Field = "Text", Operator = "ct", Value = "e" }
            };

            // act
            query = query.Filter(filters);
            var result = query.ToList();

            // assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void FilterNotesShouldReturnNotesFilteredByFromDate()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 1);
            var filters = new Filter[] {
                new Filter() { Field = "FromDate", Operator = "ge", Value = DateTime.Now.AddDays(-5).ToString() },
                new Filter() { Field = "FromDate", Operator = "le", Value = DateTime.Now.AddDays(3).ToString() }
            };

            // act
            query = query.Filter(filters);
            var result = query.OrderBy(x => x.Name).ToList();

            // assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Moiraine", result[0].Name);
            Assert.Equal("Nynaeve", result[1].Name);
        }

        [Fact]
        public void FilterNotesShouldReturnNotesFilteredByFromDateAndToDate()
        {
            // arrange
            var query = _context.Notes.Where(x => x.CalendarId == 1);
            var filters = new Filter[] {
                new Filter() { Field = "FromDate", Operator = "ge", Value = DateTime.Now.AddDays(2).ToString() },
                new Filter() { Field = "ToDate", Operator = "gt", Value = DateTime.Now.AddDays(3).ToString() }
            };

            // act
            query = query.Filter(filters);
            var result = query.OrderBy(x => x.Name).ToList();

            // assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Lanfear", result[0].Name);
            Assert.Equal("Moiraine", result[1].Name);
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