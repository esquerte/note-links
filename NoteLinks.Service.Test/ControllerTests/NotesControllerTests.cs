using System;
using Xunit;
using Moq;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.Controllers;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Linq.Expressions;
using NoteLinks.Data.Models;
using NoteLinks.Data.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using NoteLinks.Service.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace NoteLinks.Service.Test
{
    public class NotesControllerTests
    {
        private ILogger<NotesController> _logger;
        private IMapper _mapper;
        private List<Note> _noteList;

        public NotesControllerTests()
        {
            _logger = Mock.Of<ILogger<NotesController>>();

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
                .CreateMapper();

            _noteList = new List<Note>()
            {
                new Note { Id = 1, Name = "Egwene", FromDate = DateTime.Now.AddHours(2), Text = "Blue", CalendarId = 1 },
                new Note { Id = 2, Name = "Lanfear", FromDate = DateTime.Now.AddHours(3), Text = "", CalendarId = 1 },
                new Note { Id = 3, Name = "Nynaeve", FromDate = DateTime.Now.AddDays(1), Text = "", CalendarId = 1 },
                new Note { Id = 4, Name = "Moiraine", FromDate = DateTime.Now.AddMinutes(5), Text = "Blue", CalendarId = 1 },
                new Note { Id = 5, Name = "Elayne", FromDate = DateTime.Now.AddHours(6), Text = "Green", CalendarId = 2 },
                new Note { Id = 6, Name = "Cadsuane", FromDate = DateTime.Now.AddDays(3), Text = "Green", CalendarId = 2 },
                new Note { Id = 7, Name = "Min", FromDate = DateTime.Now.AddMinutes(15), Text = "", CalendarId = 2 },
            };
        }
        [Fact]
        public async void GetShouldReturnAllRequestedNotes()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var noteRepositoryMock = new Mock<INoteRepository>();
            var pageInfo = Mock.Of<PageInfo>();
            var filters = new Filter[2];

            noteRepositoryMock.Setup(x => x.GetNotesAsync(It.IsAny<Expression<Func<Note, bool>>>(), It.IsAny<Filter[]>(), It.IsAny<PageInfo>()))
                .ReturnsAsync(_noteList);

            noteRepositoryMock.Setup(x => x.GetNotesCountAsync(It.IsAny<Expression<Func<Note, bool>>>(), It.IsAny<Filter[]>()))
                .ReturnsAsync(_noteList.Count);
            
            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Get("calendarCode", filters, pageInfo) as ObjectResult;

            // assert

            Assert.IsType<ResultNoteModel>(result.Value);
            Assert.Equal(7, (result.Value as ResultNoteModel).Notes.Count);
        }

        [Fact]
        public async void GetShouldReturnNotFoundResultIfEmptyCalendarCodePassed()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var noteRepositoryMock = new Mock<INoteRepository>();
            var pageInfo = Mock.Of<PageInfo>();
            var filters = new Filter[2];

            noteRepositoryMock.Setup(x => x.GetNotesAsync(It.IsAny<Expression<Func<Note, bool>>>(), It.IsAny<Filter[]>(), It.IsAny<PageInfo>()))
                .ReturnsAsync(_noteList);

            noteRepositoryMock.Setup(x => x.GetNotesCountAsync(It.IsAny<Expression<Func<Note, bool>>>(), It.IsAny<Filter[]>()))
                .ReturnsAsync(_noteList.Count);

            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Get("", filters, pageInfo);

            // assert

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void PostShouldReturnCreatedNote()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();
            var noteRepositoryMock = new Mock<INoteRepository>();

            var createNoteModel = new CreateNoteModel()
            {
                CalendarCode = "calendarCode",
                Name = "Siuan",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddMonths(7),
                Text = "Blue"
            };

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(new Calendar() { Id = 2 });

            noteRepositoryMock.Setup(x => x.Add(It.IsAny<Note>())).Callback<Note>(x =>
            {
                x.Id = 8;
                _noteList.Add(x);
            });

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Post(createNoteModel) as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<NoteModel>(result.Value);
            Assert.Equal("Siuan", (result.Value as NoteModel).Name);
            Assert.Contains(_noteList, x => x.Id == 8);
            Assert.Equal(2, _noteList.SingleOrDefault(x => x.Id == 8).CalendarId);
        }

        [Fact]
        public async void PostShouldReturnNotFoundResultIfCalendarIsNotFoundByPassedCalendarCode()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();
            var noteRepositoryMock = new Mock<INoteRepository>();

            var createNoteModel = new CreateNoteModel()
            {
                CalendarCode = "calendarCode",
                Name = "Siuan",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddMonths(7),
                Text = "Blue"
            };

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .Returns(Task.FromResult<Calendar>(null));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Post(createNoteModel);

            // assert

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void PutShouldReturnUpdatedNote()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var noteRepositoryMock = new Mock<INoteRepository>();

            int updatedNoteId = 0;

            var noteModel = new NoteModel()
            {
                Id = 3,
                Name = "Nynaeve al'Meara",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddMinutes(9),
                Text = "Yellow"
            };

            noteRepositoryMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(_noteList.SingleOrDefault(x => x.Id == noteModel.Id));

            noteRepositoryMock.Setup(x => x.Update(It.IsAny<Note>())).Callback<Note>(x =>
            {
                updatedNoteId = x.Id;
            });

            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Put(noteModel) as ObjectResult;

            // assert
            
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<NoteModel>(result.Value);
            Assert.Equal(3, updatedNoteId);
            Assert.Equal("Nynaeve al'Meara", (result.Value as NoteModel).Name);
            Assert.Equal("Yellow", (result.Value as NoteModel).Text);
        }

        [Fact]
        public async void PutShouldUpdatePropertyToDefaultValueIfItIsNotExistsInPassedModel()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var noteRepositoryMock = new Mock<INoteRepository>();

            int updatedNoteId = 0;

            var noteModel = new NoteModel()
            {
                Id = 3,
                Name = "Nynaeve al'Meara",
            };

            noteRepositoryMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(_noteList.SingleOrDefault(x => x.Id == noteModel.Id));

            noteRepositoryMock.Setup(x => x.Update(It.IsAny<Note>())).Callback<Note>(x =>
            {
                updatedNoteId = x.Id;
            });

            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Put(noteModel) as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<NoteModel>(result.Value);
            Assert.Equal(3, updatedNoteId);
            Assert.Equal("Nynaeve al'Meara", (result.Value as NoteModel).Name);
            Assert.Equal(DateTime.MinValue, (result.Value as NoteModel).FromDate);
            Assert.Null((result.Value as NoteModel).ToDate);
            Assert.Null((result.Value as NoteModel).Text);
        }

        [Fact]
        public async void PutShouldReturnNotFoundResultIfThereIsNoNoteForUpdate()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var noteRepositoryMock = new Mock<INoteRepository>();

            var noteModel = new NoteModel()
            {
                Id = 8,
                Name = "Nynaeve al'Meara",
            };

            noteRepositoryMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(_noteList.SingleOrDefault(x => x.Id == noteModel.Id));

            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Put(noteModel);

            // assert

            Assert.DoesNotContain(_noteList, x => x.Id == 8);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void DeleteShouldReturnOkResultIfExistingNoteIdPassed()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var noteRepositoryMock = new Mock<INoteRepository>();

            int noteId = 2;

            noteRepositoryMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(_noteList.SingleOrDefault(x => x.Id == noteId));

            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Delete(noteId);

            // assert

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void DeleteShouldReturnNotFoundResultIfNotExistingNoteIdPassed()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var noteRepositoryMock = new Mock<INoteRepository>();

            int noteId = 8;

            noteRepositoryMock.Setup(x => x.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(_noteList.SingleOrDefault(x => x.Id == noteId));

            unitOfWorkMock.Setup(x => x.Notes).Returns(noteRepositoryMock.Object);

            var controller = new NotesController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Delete(noteId);

            // assert

            Assert.DoesNotContain(_noteList, x => x.Id == 8);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
