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
using NoteLinks.Service.ExceptionHandling;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NoteLinks.Service.Test.ControllerTests;

namespace NoteLinks.Service.Test.ControllerTests
{
    public class CalendarsControllerTests
    {
        private ILogger<CalendarsController> _logger;
        private IMapper _mapper;
        private Mock<FakeUserManager> _userManagerMock;

        public CalendarsControllerTests()
        {
            _logger = Mock.Of<ILogger<CalendarsController>>();

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
                .CreateMapper();

            _userManagerMock = new Mock<FakeUserManager>();
        }

        [Fact]
        public async void GetShouldReturnRequestedCalendar()
        {
            // arrange

            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string calendarCode = "asdf";

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendars.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            var result = await controller.Get(calendarCode) as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CalendarModel>(result.Value);
            Assert.Equal("Birthdays", (result.Value as CalendarModel).Name);
        }

        [Fact]
        public async void GetShouldReturnApiExceptionIfNotExistingCalendarCodePassed()
        {
            // arrange

            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string calendarCode = "uiop";

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendars.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);


            // act

            Func<Task> result = () => controller.Get(calendarCode);

            // assert

            Assert.DoesNotContain(calendars, x => x.Code == "uiop");
            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Theory]
        [InlineData(4)]
        public async void PostShouldReturnCreatedCalendar(int calendarId)
        {
            // arrange

            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();            
            var calendarRepositoryMock = new Mock<ICalendarRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();

            var createCalendarModel = new CreateCalendarModel()
            {
                Name = "Mortgage payments"
            };
                      
            calendarRepositoryMock.Setup(x => x.Add(It.IsAny<Calendar>())).Callback<Calendar>(x =>
            {
                x.Id = calendarId;
                calendars.Add(x);
            });

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            var result = await controller.Post(createCalendarModel) as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CalendarModel>(result.Value);
            Assert.Equal("Mortgage payments", (result.Value as CalendarModel).Name);
            Assert.NotNull((result.Value as CalendarModel).Code);
            Assert.Contains(calendars, x => x.Id == calendarId);
        }

        [Theory]
        [InlineData(4)]
        public async void PostShouldAddCreatorIfUserAuthenticated(int calendarId)
        {
            // arrange

            List<Calendar> calendars = GetCalendars();
            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();

            var createCalendarModel = new CreateCalendarModel()
            {
                Name = "Mortgage payments"
            };

            calendarRepositoryMock.Setup(x => x.Add(It.IsAny<Calendar>())).Callback<Calendar>(x =>
            {
                x.Id = calendarId;
                calendars.Add(x);
            });

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.AddUserCalendarAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(0);

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            var result = await controller.Post(createCalendarModel) as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CalendarModel>(result.Value);
            Assert.Equal("Mortgage payments", (result.Value as CalendarModel).Name);
            Assert.NotNull((result.Value as CalendarModel).Code);
            Assert.Contains(calendars, x => x.Id == calendarId);
            Assert.Equal(user.Email, calendars.Single(x => x.Id == calendarId).Creator.Email);
        }

        [Fact]
        public async void PutShouldReturnUpdatedCalendar()
        {
            // arrange

            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            string updatedCalendarCode = null;

            var calendarModel = new CalendarModel()
            {
                Code = "zxcv",
                Name = "Official meetings"
            };

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendars.SingleOrDefault(x => x.Code == calendarModel.Code));

            calendarRepositoryMock.Setup(x => x.Update(It.IsAny<Calendar>())).Callback<Calendar>(x =>
            {
                updatedCalendarCode = x.Code;
            });

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

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

            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            var calendarModel = new CalendarModel()
            {
                Code = "uiop",
                Name = "Official meetings"
            };

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendars.SingleOrDefault(x => x.Code == calendarModel.Code));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Put(calendarModel);

            // assert

            Assert.DoesNotContain(calendars, x => x.Code == "uiop");
            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Theory]
        [InlineData("asdf")]
        public async void DeleteShouldReturnOkResultIfExistingCalendarCodePassed(string calendarCode)
        {
            // arrange

            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendars.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            var result = await controller.Delete(calendarCode);

            // assert

            Assert.IsType<OkResult>(result);
        }

        [Theory]
        [InlineData("uiop")]
        public async void DeleteShouldReturnApiExceptionIfNotExistingCalendarCodePassed(string calendarCode)
        {
            // arrange

            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendars.SingleOrDefault(x => x.Code == calendarCode));

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Delete(calendarCode);

            // assert

            Assert.DoesNotContain(calendars, x => x.Code == "uiop");
            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Theory]
        [InlineData("qwer")]
        public async void DeleteShouldReturnApiExceptionIfCalendarIsNotCreatedByUser(string calendarCode)
        {
            // arrange

            List<Calendar> calendars = GetCalendars();
            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendars.SingleOrDefault(x => x.Code == calendarCode));

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.AddUserCalendarAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(0);

            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new CalendarsController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Delete(calendarCode);

            // assert

            await Assert.ThrowsAsync<ApiException>(result);            
        }

        private List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = "ee38f91b-2027-41f5-ac0f-d2f8246e3a0c",
                    Email = "adam@heaven.ru",
                    UserName = "adam@heaven.ru",
                },
                new User
                {
                    Id = "7ad8bb6e-aa99-4bfb-9318-78064d893769",
                    Email = "eva@heaven.ru",
                    UserName = "eva@heaven.ru",
                }            
            };
        }

        private List<Calendar> GetCalendars()
        {
            List<User> users = GetUsers();

            return new List<Calendar>()
            {
                new Calendar { Id = 1, Name = "Birthdays", Code = "asdf" },
                new Calendar { Id = 2, Name = "Vacations", Code = "qwer",
                    Creator = users.Single(x => x.Email == "eva@heaven.ru")
                },
                new Calendar { Id = 3, Name = "Meetings", Code = "zxcv" }
            };
        }

    }
    
}
