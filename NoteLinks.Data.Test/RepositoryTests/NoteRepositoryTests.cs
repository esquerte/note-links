using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using NoteLinks.Data.Repository.Implementations;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using Xunit;

namespace NoteLinks.Data.Test
{
    public class NoteRepositoryTests
    {
        private IUnitOfWork _unitOfWork;
        private INoteRepository _noteRepository;
        private List<Note> _noteList; 

        public NoteRepositoryTests()
        {
            _unitOfWork = GetUnitOfWork();
            _noteRepository = _unitOfWork.Notes;

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

            _noteRepository.AddRange(_noteList);
            _unitOfWork.CompleteAsync();
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesOfSpecificCalendar()
        {
            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => x.CalendarId == 1, null, null);

            // assert
            Assert.NotEmpty(result);
            Assert.DoesNotContain(result, x => x.CalendarId != 1);
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesOrderedByNameDescending()
        {
            // arrange
            var pageInfo = new PageInfo()
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Name",
                Desc = true
            };

            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => x.CalendarId == 2, null, pageInfo);

            // assert
            Assert.NotEmpty(result);
            Assert.Equal("Min", result[0].Name);
            Assert.Equal("Elayne", result[1].Name);
            Assert.Equal("Cadsuane", result[2].Name);
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesFilteredByNameContains()
        {
            // arrange
            var filters = new Filter[] {
                new Filter() { Field = "Name", Operator = "ct", Value = "ne" }
            };

            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => true, filters, null);

            // assert
            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count);
        }


        [Fact]
        public async void GetNotesAsyncShouldReturnNotesFromSecondPageFilteredByFromDateAndOrderedByFromDateDescending()
        {
            // arrange
            var filters = new Filter[] {
                new Filter() { Field = "FromDate", Operator = "ge", Value = DateTime.Now.AddDays(-5).ToString() },
                new Filter() { Field = "FromDate", Operator = "le", Value = DateTime.Now.AddDays(4).ToString() }
            };
            var pageInfo = new PageInfo()
            {
                PageSize = 3,
                PageIndex = 2,
                OrderBy = "FromDate",
                Desc = true
            };

            // act
            List<Note> result = await _noteRepository.GetNotesAsync(x => true, filters, pageInfo);

            // assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Min", result[0].Name);
            Assert.Equal("Nynaeve", result[1].Name);
        }

        private UnitOfWork GetUnitOfWork()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "NoteTestDatabase")
                .Options;
            var context = new MainContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return new UnitOfWork(context);
        }
    }
}
