using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Implementations;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NoteLinks.Data.Test.RepositoryTests
{
    // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory#get-your-context-ready
    public class UserRepositoryTests
    {
        public UserRepositoryTests()
        {        
        }

        [Fact]
        public async void GetUserCalendarsAsyncShouldReturnCalendarsOfSpecificUser()
        {
            var databaseName = Guid.NewGuid().ToString();

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                SeedData(context);
            }

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                var repository = new UserRepository(context);
                UserManager<User> userManager = GetUserManager(context);

                User Adam = await userManager.FindByEmailAsync("adam@heaven.ru");

                // act

                List<Calendar> result = await repository.GetUserCalendarsAsync(Adam.Id);

                // assert

                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(result, x => x.Id == 2);
                Assert.Contains(result, x => x.Id == 3);
                Assert.Contains(result, x => x.Id == 3 && x.Creator != null);
            }

        }

        [Fact]
        public async void AddUserCalendarAsyncShouldAddCalendarForSpecificUser()
        {
            var databaseName = Guid.NewGuid().ToString();
            int calendarId = 1;

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                SeedData(context);
            }

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                var repository = new UserRepository(context);
                UserManager<User> userManager = GetUserManager(context);

                User Adam = await userManager.FindByEmailAsync("adam@heaven.ru");

                // act

                await repository.AddUserCalendarAsync(Adam.Id, calendarId);

                // assert

                Assert.Contains(context.Calendars.SelectMany(x => x.UserCalendars),
                                x => x.UserId == Adam.Id && x.CalendarId == calendarId);
            }
        }

        [Fact]
        public async void DeleteUserCalendarAsyncShouldDeleteCalendarFromUser()
        {
            var databaseName = Guid.NewGuid().ToString();
            int calendarId = 2;

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                SeedData(context);
            }

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                var repository = new UserRepository(context);
                UserManager<User> userManager = GetUserManager(context);

                User Adam = await userManager.FindByEmailAsync("adam@heaven.ru");

                // act

                await repository.DeleteUserCalendarAsync(Adam.Id, calendarId);

                // assert

                Assert.DoesNotContain(context.Calendars.SelectMany(x => x.UserCalendars),
                                      x => x.UserId == Adam.Id && x.CalendarId == calendarId);        
            }
        }

        [Fact]
        public async void DeleteUserDataAsyncShouldDeleteCalendarsIfPassTrueForDeleteCalendars()
        {
            var databaseName = Guid.NewGuid().ToString();

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                SeedData(context);
            }

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                var repository = new UserRepository(context);
                UserManager<User> userManager = GetUserManager(context);

                User Adam = await userManager.FindByEmailAsync("adam@heaven.ru");
                bool deleteCalendars = true;

                // act

                await repository.DeleteUserDataAsync(Adam.Id, deleteCalendars);

                // assert

                Assert.DoesNotContain(context.Calendars, x => x.Id == 3);
                Assert.DoesNotContain(context.Calendars.SelectMany(x => x.UserCalendars),
                                      x => x.CalendarId == 3);
            }
        }

        [Fact]
        public async void DeleteUserDataAsyncShouldClearCreatorFieldIfPassFalseForDeleteCalendars()
        {
            var databaseName = Guid.NewGuid().ToString();

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                SeedData(context);
            }

            using (var context = GetInMemoryDataContext(databaseName))
            {
                // arrange

                var repository = new UserRepository(context);
                UserManager<User> userManager = GetUserManager(context);

                User Adam = await userManager.FindByEmailAsync("adam@heaven.ru");
                bool deleteCalendars = false;

                // act

                await repository.DeleteUserDataAsync(Adam.Id, deleteCalendars);

                // assert

                Assert.Null(context.Calendars.Single(x => x.Id == 3).Creator);
            }
        }

        private User CreateAdam(UserManager<User> userManager)
        {
            var Adam = new User
            {
                Email = "adam@heaven.ru",
                UserName = "adam@heaven.ru"
            };

            userManager.CreateAsync(Adam, "1234Fd#gr56Dffw");

            return Adam;
        }

        private User CreateEva(UserManager<User> userManager)
        {
            var Eva = new User
            {
                Email = "eva@heaven.ru",
                UserName = "eva@heaven.ru"
            };

            userManager.CreateAsync(Eva, "565f%3dfgFjlOh0H");

            return Eva;
        }

        private void SeedData(MainContext context)
        {
            var repository = new UserRepository(context);
            UserManager<User> userManager = GetUserManager(context);

            User Adam = CreateAdam(userManager);
            User Eva = CreateEva(userManager);

            var calendars = new List<Calendar>()
                {
                    new Calendar { Id = 1 },
                    new Calendar { Id = 2,
                        UserCalendars = new List<UserCalendar> {
                            new UserCalendar { User = Adam, CalendarId = 2 }
                        }
                    },
                    new Calendar { Id = 3, Creator = Adam,
                        UserCalendars = new List<UserCalendar> {
                            new UserCalendar { User = Adam, CalendarId = 3 },
                            new UserCalendar { User = Eva, CalendarId = 3 }
                        }
                    }
                };

            context.Calendars.AddRange(calendars);
            context.SaveChanges();
        }

        private UserManager<User> GetUserManager(MainContext context)
        {
            var userStore = new UserStore<User>(context);
            IPasswordHasher<User> hasher = new PasswordHasher<User>();
            var validator = new UserValidator<User>();
            var validators = new List<UserValidator<User>> { validator };

            return new UserManager<User>(userStore, null, hasher, validators,
                null, null, null, null, null);
        }

        private MainContext GetInMemoryDataContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                            .UseInMemoryDatabase(databaseName)
                            .Options;
            return new MainContext(options);
        }

    }
}
