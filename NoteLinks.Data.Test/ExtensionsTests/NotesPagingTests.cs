using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Extensions;
using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NoteLinks.Data.Test.ExtensionsTests
{
    public class NotesPagingTests
    {
        public NotesPagingTests()
        {        
        }

        [Fact]
        public void PagingNotesShouldNotChangeQueryIfPageInfoIsNull()
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

                var query = context.Notes.Where(x => x.CalendarId == 1);
                PageInfo pageInfo = null;

                // act

                query = query.Paginate(pageInfo);
                List<Note> result = query.ToList();

                // assert

                Assert.Equal(4, result.Count);
            }
        }

        [Fact]
        public void PagingNotesShouldReturnNotesOrderedByNameDescending()
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

                var query = context.Notes.Where(x => x.CalendarId == 2);
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
        }

        [Fact]
        public void PagingNotesShouldReturnNotesOrderedByFromDate()
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

                var query = context.Notes.Where(x => x.CalendarId == 1);
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
        }

        [Fact]
        public void PagingNotesShouldReturnNotesFromSecondPage()
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
                var query = context.Notes.Where(x => x.CalendarId == 2);
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
