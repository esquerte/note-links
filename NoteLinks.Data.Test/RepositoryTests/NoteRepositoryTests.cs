using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using NoteLinks.Data.Repository.Implementations;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NoteLinks.Data.Test.RepositoryTests
{
    public class NoteRepositoryTests
    {
        public NoteRepositoryTests()
        {
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesOfSpecificCalendar()
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

                var repository = new NoteRepository(context);

                // act

                List<Note> result = await repository.GetNotesAsync(x => x.CalendarId == 1, null, null);

                // assert

                Assert.NotEmpty(result);
                Assert.DoesNotContain(result, x => x.CalendarId != 1);
            }
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesOrderedByNameDescending()
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

                var repository = new NoteRepository(context);

                var pageInfo = new PageInfo()
                {
                    PageIndex = 1,
                    PageSize = 10,
                    OrderBy = "Name",
                    Desc = true
                };

                // act

                List<Note> result = await repository.GetNotesAsync(x => x.CalendarId == 2, null, pageInfo);

                // assert

                Assert.NotEmpty(result);
                Assert.Equal("Min", result[0].Name);
                Assert.Equal("Elayne", result[1].Name);
                Assert.Equal("Cadsuane", result[2].Name);
            }
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesFilteredByNameContains()
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

                var repository = new NoteRepository(context);

                var filters = new Filter[] {
                    new Filter() { Field = "Name", Operator = "ct", Value = "ne" }
                };

                // act

                List<Note> result = await repository.GetNotesAsync(x => true, filters, null);

                // assert

                Assert.NotEmpty(result);
                Assert.Equal(4, result.Count);
            }
        }

        [Fact]
        public async void GetNotesAsyncShouldReturnNotesFromSecondPageFilteredByFromDateAndOrderedByFromDateDescending()
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

                var repository = new NoteRepository(context);

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

                List<Note> result = await repository.GetNotesAsync(x => true, filters, pageInfo);

                // assert

                Assert.Equal(2, result.Count);
                Assert.Equal("Min", result[0].Name);
                Assert.Equal("Nynaeve", result[1].Name);
            }
        }

        private void SeedData(MainContext context)
        {
            var calendars = new List<Calendar>()
            {
                new Calendar { Id = 1,
                    Notes = new List<Note> {
                        new Note { Id = 1, Name = "Egwene", FromDate = DateTime.Now.AddDays(-7), ToDate = DateTime.Now.AddDays(5), Text = "Blue" },
                        new Note { Id = 2, Name = "Lanfear", FromDate = DateTime.Now.AddDays(5), ToDate = DateTime.Now.AddDays(6), Text = "" },
                        new Note { Id = 3, Name = "Nynaeve", FromDate = DateTime.Now.AddDays(-4), Text = "Yellow" },
                        new Note { Id = 4, Name = "Moiraine", FromDate = DateTime.Now.AddDays(2), ToDate = DateTime.Now.AddDays(4), Text = "Blue" },
                    }
                },
                new Calendar { Id = 2,
                    Notes = new List<Note> {
                        new Note { Id = 5, Name = "Elayne", FromDate = DateTime.Now.AddHours(6), Text = "Green" },
                        new Note { Id = 6, Name = "Cadsuane", FromDate = DateTime.Now.AddDays(3), Text = "Green" },
                        new Note { Id = 7, Name = "Min", FromDate = DateTime.Now.AddMinutes(15), Text = "" },
                    }
                }
            };

            context.Calendars.AddRangeAsync(calendars);
            context.SaveChanges();
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
