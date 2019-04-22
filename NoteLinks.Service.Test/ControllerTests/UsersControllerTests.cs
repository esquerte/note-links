using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.Controllers;
using NoteLinks.Service.ExceptionHandling;
using NoteLinks.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NoteLinks.Service.Test.ControllerTests
{
    public class UsersControllerTests
    {
        private ILogger<UsersController> _logger;
        private IMapper _mapper;
        private Mock<FakeUserManager> _userManagerMock;

        public UsersControllerTests()
        {
            _logger = Mock.Of<ILogger<UsersController>>();

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
                .CreateMapper();

            _userManagerMock = new Mock<FakeUserManager>();
        }

        [Fact]
        public async void AuthenticateShouldReturnTokenIfUserExistsAndPasswordIsValid()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            var result = await controller.Post(new AuthenticationModel()) as ObjectResult;

            // assert

            Assert.IsType<AuthenticationResultModel>(result.Value);
            Assert.NotNull((result.Value as AuthenticationResultModel).AccessToken);
            Assert.Equal("Adam", (result.Value as AuthenticationResultModel).DisplayName);
        }

        [Fact]
        public async void AuthenticateShouldReturnApiExceptionIfUserDoesNotExist()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Post(new AuthenticationModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void AuthenticateShouldReturnApiExceptionIfPasswordIsNotValid()
        {
            // arrange

            User user = null;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Post(new AuthenticationModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void PostShouldReturnOkResultIfUserHasBeenCreated()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            var result = await controller.Post(new CreateUserModel());

            // assert

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void PostShouldReturnApiExceptionIfUserHasNotBeenCreated()
        {
            // arrange

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);

            // act

            Func<Task> result = () => controller.Post(new CreateUserModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void DeleteShouldReturnOkResultIfUserHasBeenDeleted()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            userRepositoryMock.Setup(x => x.DeleteUserDataAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(0);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            var result = await controller.Post(new DeleteUserModel());

            // assert

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void DeleteShouldReturnApiExceptionIfUserHasNotBeenDeleted()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed());

            userRepositoryMock.Setup(x => x.DeleteUserDataAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(0);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Post(new DeleteUserModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void DeleteShouldReturnApiExceptionIfUserDoesNotExist()
        {
            // arrange

            User user = null;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Post(new DeleteUserModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void DeleteShouldReturnApiExceptionIfPasswordIsNotValid()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Post(new DeleteUserModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void GetCalendarsShouldReturnUserCalendars()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");
            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.GetUserCalendarsAsync(It.IsAny<string>()))
                .ReturnsAsync(calendars);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            var result = await controller.Get() as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<UserCalendarModel>>(result.Value);
            Assert.Equal(3, (result.Value as List<UserCalendarModel>).Count());
        }

        [Fact]
        public async void GetCalendarsShouldSetCreatorToTrueIfCalendarWasCreatedByCurrentUser()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");
            List<Calendar> calendars = GetCalendars();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.GetUserCalendarsAsync(It.IsAny<string>()))
                .ReturnsAsync(calendars);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            var result = await controller.Get() as ObjectResult;

            // assert

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<UserCalendarModel>>(result.Value);
            Assert.Equal(3, (result.Value as List<UserCalendarModel>).Count());
            Assert.Equal("Vacations", (result.Value as List<UserCalendarModel>).Single(x => x.Creator).Name);
        }

        [Fact]
        public async void GetCalendarsShouldReturnApiExceptionIfUserDoesNotExist()
        {
            // arrange

            User user = null;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Get();

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void PostCalendarShouldReturnOkResultIfCalendarHasBeenAdded()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");
            Calendar calendar = GetCalendars().Single(x => x.Code == "asdf");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.AddUserCalendarAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(0);

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendar);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            var result = await controller.Post(new CreateUserCalendarModel());

            // assert

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void PostCalendarShouldReturnApiExceptionIfUserDoesNotExist()
        {
            // arrange

            User user = null;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Post(new CreateUserCalendarModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void PostCalendarShouldReturnApiExceptionIfCalendarDoesNotExist()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");
            Calendar calendar = null;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendar);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Post(new CreateUserCalendarModel());

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void DeleteCalendarShouldReturnOkResultIfCalendarHasBeenDeleted()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");
            Calendar calendar = GetCalendars().Single(x => x.Code == "asdf");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.DeleteUserCalendarAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(0);

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendar);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            var result = await controller.Delete("asdf");

            // assert

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void DeleteCalendarShouldReturnApiExceptionIfUserDoesNotExist()
        {
            // arrange

            User user = null;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Delete("asdf");

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void DeleteCalendarShouldReturnApiExceptionIfCalendarDoesNotExist()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");
            Calendar calendar = null;

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendar);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Delete("asdf");

            // assert

            await Assert.ThrowsAsync<ApiException>(result);
        }

        [Fact]
        public async void DeleteCalendarShouldReturnApiExceptionIfCalendarIsCreatedByUser()
        {
            // arrange

            User user = GetUsers().Single(x => x.Email == "adam@heaven.ru");
            Calendar calendar = GetCalendars().Single(x => x.Code == "qwer");

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var calendarRepositoryMock = new Mock<ICalendarRepository>();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            userRepositoryMock.Setup(x => x.DeleteUserCalendarAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(0);

            calendarRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
                .ReturnsAsync(calendar);

            unitOfWorkMock.Setup(x => x.Users).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.Calendars).Returns(calendarRepositoryMock.Object);

            var controller = new UsersController(unitOfWorkMock.Object, _userManagerMock.Object, _logger, _mapper);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // act

            Func<Task> result = () => controller.Delete("qwer");

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
                    DisplayName = "Adam"
                },
                new User
                {
                    Id = "7ad8bb6e-aa99-4bfb-9318-78064d893769",
                    Email = "eva@heaven.ru",
                    UserName = "eva@heaven.ru",
                    DisplayName = "Eva"
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
                    Creator = users.Single(x => x.Email == "adam@heaven.ru")
                },
                new Calendar { Id = 3, Name = "Meetings", Code = "zxcv" }
            };
        }

        private UserManager<User> GetUserManager()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "adam@heaven.ru",
                    UserName = "adam@heaven.ru",
                    DisplayName = "Adam"
                },

                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "eva@heaven.ru",
                    UserName = "eva@heaven.ru",
                    DisplayName = "Eva"
                }

            }.AsQueryable();

            var userManagerMock = new Mock<FakeUserManager>();

            //userManagerMock.Setup(x => x.Users)
            //    .Returns(users);

            userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(users.Single(x => x.Email == "adam@heaven.ru"));
            //userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            //    .ReturnsAsync(IdentityResult.Success);
            //userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            //    .ReturnsAsync(IdentityResult.Success);
            //userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<User>()))
            //    .ReturnsAsync(IdentityResult.Success);

            return userManagerMock.Object;
        }
    }
}