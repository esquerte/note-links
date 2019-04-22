using NoteLinks.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<Calendar>> GetUserCalendarsAsync(string userId);
        Task<int> AddUserCalendarAsync(string userId, int calendarId);
        //Task<int> DeleteUserCalendarAsync2(User user, Calendar calendar);
        Task<int> DeleteUserCalendarAsync(string userId, int calendarId);
        //Task<bool> DeleteUserAsync(User user, bool deleteCalendars);
        Task<int> DeleteUserDataAsync(string userId, bool deleteCalendars);
    }
}
