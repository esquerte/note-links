using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        public NoteRepository(MainDbContext context)
            : base(context)
        {
        }

        //public IEnumerable<Course> GetTopSellingCourses(int count)
        //{
        //    return PlutoContext.Courses.OrderByDescending(c => c.FullPrice).Take(count).ToList();
        //}

        //public IEnumerable<Course> GetCoursesWithAuthors(int pageIndex, int pageSize = 10)
        //{
        //    return PlutoContext.Courses
        //        .Include(c => c.Author)
        //        .OrderBy(c => c.Name)
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();
        //}

        MainDbContext CalendarContext => Context as MainDbContext;
    }
}
