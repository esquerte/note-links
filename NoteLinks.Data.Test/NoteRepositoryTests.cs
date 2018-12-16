using System;
using Xunit;
using Moq;
using NoteLinks.Data.Models;
using NoteLinks.Data.Repository.Interfaces;
using System.Collections.Generic;
using NoteLinks.Data.Entities;
using System.Linq;
using NoteLinks.Data.Repository.Implementations;
using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;

namespace NoteLinks.Data.Test
{
    public class NoteRepositoryTests
    {
        private PageInfo _pageInfo;
        private INoteRepository _noteRepository;
        private List<Note> _noteList; 

        public NoteRepositoryTests()
        {
            _pageInfo = new PageInfo();
            _noteRepository = GetInMemoryNoteRepository();

            _noteList = new List<Note>()
            {
                new Note { Id = 1, Name = "Egwene", FromDate = DateTime.Now.AddHours(2), Text = "Blue", CalendarId = 1 },
                new Note { Id = 2, Name = "Lanfear", FromDate = DateTime.Now.AddHours(3), Text = "", CalendarId = 1 },
                new Note { Id = 3, Name = "Nynaeve", FromDate = DateTime.Now.AddDays(1), Text = "Yellow", CalendarId = 1 },
                new Note { Id = 4, Name = "Moiraine", FromDate = DateTime.Now.AddMinutes(5), Text = "Blue", CalendarId = 1 },
                new Note { Id = 5, Name = "Elayne", FromDate = DateTime.Now.AddHours(6), Text = "Green", CalendarId = 2 },
                new Note { Id = 6, Name = "Cadsuane", FromDate = DateTime.Now.AddDays(3), Text = "Green", CalendarId = 2 },
                new Note { Id = 7, Name = "Min", FromDate = DateTime.Now.AddMinutes(15), Text = "", CalendarId = 2 },
            };
            
            _noteRepository.AddRange(_noteList);
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesOfSpecificCalendar()
        {
            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => x.CalendarId == 1, _pageInfo);

            // assert
            Assert.NotEmpty(result);
            Assert.DoesNotContain(result, x => x.CalendarId != 1);
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesOrderedByNameDescending()
        {
            // arrange
            _pageInfo.OrderBy = "Name";
            _pageInfo.Desc = true;

            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => x.CalendarId == 2, _pageInfo);

            // assert
            Assert.NotEmpty(result);
            Assert.Equal("Min", result[0].Name);
            Assert.Equal("Elayne", result[1].Name);
            Assert.Equal("Cadsuane", result[2].Name);
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesOrderedByFromDate()
        {
            // arrange
            _pageInfo.OrderBy = "FromDate";

            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => x.CalendarId == 1, _pageInfo);

            // assert
            Assert.NotEmpty(result);
            Assert.Equal("Moiraine", result[0].Name);
            Assert.Equal("Egwene", result[1].Name);
            Assert.Equal("Lanfear", result[2].Name);
            Assert.Equal("Nynaeve", result[3].Name);
        }     

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesFromSecondPage()
        {
            // arrange
            _pageInfo.OrderBy = "FromDate";
            _pageInfo.PageIndex = 2;
            _pageInfo.PageSize = 2;

            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => x.CalendarId == 2, _pageInfo);

            // assert
            Assert.NotEmpty(result);
            Assert.Equal("Cadsuane", result[0].Name);
        }

        private INoteRepository GetInMemoryNoteRepository()
        {
            var options = new DbContextOptionsBuilder<MainDataContext>()
                .UseInMemoryDatabase(databaseName: "NoteTestDatabase")
                .Options;
            var mainDataContext = new MainDataContext(options);
            mainDataContext.Database.EnsureDeleted();
            mainDataContext.Database.EnsureCreated();
            return new NoteRepository(mainDataContext);
        }
    }
}
