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
using NoteLinks.Service.ExceptionFilter;

namespace NoteLinks.Service.Test
{
    public class CalendarsControllerTests
    {
        private ILogger<CalendarsController> _logger;
        private IMapper _mapper;
        private List<Calendar> _calendarList;

        public CalendarsControllerTests()
        {
            _logger = Mock.Of<ILogger<CalendarsController>>();

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
                .CreateMapper();

            _calendarList = new List<Calendar>()
            {
                new Calendar { Id = 1, Name = "Birthdays", Code = "asdf" },
                new Calendar { Id = 2, Name = "Vacations", Code = "qwer" },
                new Calendar { Id = 3, Name = "Meetings", Code = "zxcv" }
            };
        }

        [Fact]
        public async void GetShouldReturnRequestedCalendar()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string calendarCode = "asdf";

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(_calendarList.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Get(calendarCode) as ObjectResult;

            // assert

            Assert.IsType<CalendarModel>(result.Value);
            Assert.Equal("Birthdays", (result.Value as CalendarModel).Name);
        }

        [Fact]
        public async void GetShouldReturnApiExceptionIfNotExistingCalendarCodePassed()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string calendarCode = "uiop";

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(_calendarList.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Get(calendarCode);

            // assert

            Assert.DoesNotContain(_calendarList, x => x.Code == "uiop");
            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void PostShouldReturnCreatedCalendar()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            var createCalendarModel = new CreateCalendarModel() { Name = "Mortgage payments" };

            calendarRepositoryMock.Setup(x => x.Add(It.IsAny<Calendar>())).Callback<Calendar>(x =>
            {
                x.Id = 4;
                _calendarList.Add(x);
            });

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Post(createCalendarModel) as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CalendarModel>(result.Value);
            Assert.Equal("Mortgage payments", (result.Value as CalendarModel).Name);
            Assert.NotNull((result.Value as CalendarModel).Code);
            Assert.Contains(_calendarList, x => x.Id == 4);
        }

        [Fact]
        public async void PutShouldReturnUpdatedCalendar()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string updatedCalendarCode = null;

            var calendarModel = new CalendarModel()
            {
                Code = "zxcv",
                Name = "Official meetings"
            };

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(_calendarList.SingleOrDefault(x => x.Code == calendarModel.Code));

            calendarRepositoryMock.Setup(x => x.Update(It.IsAny<Calendar>())).Callback<Calendar>(x =>
            {
                updatedCalendarCode = x.Code;
            });

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Put(calendarModel) as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CalendarModel>(result.Value);
            Assert.Equal("zxcv", updatedCalendarCode);
            Assert.Equal("Official meetings", (result.Value as CalendarModel).Name);
        }

        [Fact]
        public async void PutShouldReturnApiExceptionIfThereIsNoCalendarForUpdate()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            var calendarModel = new CalendarModel()
            {
                Code = "uiop",
                Name = "Official meetings"
            };

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(_calendarList.SingleOrDefault(x => x.Code == calendarModel.Code));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Put(calendarModel);

            // assert

            Assert.DoesNotContain(_calendarList, x => x.Code == "uiop");
            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void DeleteShouldReturnOkResultIfExistingCalendarCodePassed()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string calendarCode = "asdf";

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(_calendarList.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            var result = await controller.Delete(calendarCode);

            // assert

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void DeleteShouldReturnApiExceptionIfNotExistingCalendarCodePassed()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string calendarCode = "uiop";

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(_calendarList.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Delete(calendarCode);

            // assert

            Assert.DoesNotContain(_calendarList, x => x.Code == "uiop");
            await Assert.ThrowsAsync<ApiException>(result);
        }

    }
}
