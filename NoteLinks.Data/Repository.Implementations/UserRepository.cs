using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    // Maybe better to move some of this methods in a separate business layer
    public class UserRepository: IUserRepository
    {
        private MainContext db;

        public UserRepository(DbContext context)
        {
            db = context as MainContext;
        }

        public Task<List<Calendar>> GetUserCalendarsAsync(string userId)
        {
            var query = db.Calendars.AsNoTracking()
                        .SelectMany(x => x.UserCalendars)
                        .Where(x => x.UserId == userId)
                        .Select(x => x.Calendar)
                        .Include(x => x.Creator);

            return query.ToListAsync();
        }

        public Task<int> AddUserCalendarAsync(string userId, int calendarId)
        {
            Calendar calendar = db.Calendars.Single(x => x.Id == calendarId);
            calendar.UserCalendars.Add(new UserCalendar { UserId = userId, CalendarId = calendarId });

            return db.SaveChangesAsync();
        }

        public Task<int> DeleteUserCalendarAsync(string userId, int calendarId)
        {
            var calendar = db.Calendars
                            .Include(x => x.UserCalendars)
                            .Single(x => x.Id == calendarId);

            calendar.UserCalendars.RemoveAll(x => x.UserId == userId);

            return db.SaveChangesAsync();
        }

        public async Task<int> DeleteUserDataAsync(string userId, bool deleteCalendars)
        {
            if (deleteCalendars)
            {
                // Cascade delete from Notes and UserCalendars
                db.Calendars.RemoveRange(
                    db.Calendars.Where(x => x.Creator.Id == userId));
            }
            else
            {
                db.Calendars.AttachRange(
                    db.Calendars
                    .Where(x => x.Creator.Id == userId));

                await db.Calendars
                    .Where(x => x.Creator.Id == userId)
                    .ForEachAsync(x => x.Creator = null);
            }

            return await db.SaveChangesAsync();
        }
    }
}
