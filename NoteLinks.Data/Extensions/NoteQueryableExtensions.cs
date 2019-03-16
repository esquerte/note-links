using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Extensions
{
    public static class NoteQueryableExtensions
    {
        public static IQueryable<Note> Filter(this IQueryable<Note> query, Filter[] filters)
        {
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    switch (filter.Field)
                    {
                        case "Id":

                            switch (filter.Operator)
                            {
                                case "eq":
                                    query = query.Where(x => x.Id == Convert.ToInt32(filter.Value));
                                    break;
                            }

                            break;

                        case "Name":

                            switch (filter.Operator)
                            {
                                case "eq":
                                    query = query.Where(x => x.Name == filter.Value);
                                    break;
                                case "ct":
                                    query = query.Where(x => x.Name.Contains(filter.Value));
                                    break;
                            }

                            break;

                        case "FromDate":

                            var fromDate = new DateTime();

                            if (DateTime.TryParse(filter.Value, out fromDate))
                            {
                                switch (filter.Operator)
                                {
                                    case "eq":
                                        query = query.Where(x => x.FromDate == fromDate);
                                        break;
                                    case "lt":
                                        query = query.Where(x => x.FromDate < fromDate);
                                        break;
                                    case "le":
                                        query = query.Where(x => x.FromDate <= fromDate);
                                        break;
                                    case "gt":
                                        query = query.Where(x => x.FromDate > fromDate);
                                        break;
                                    case "ge":
                                        query = query.Where(x => x.FromDate >= fromDate);
                                        break;
                                }
                            }

                            break;

                        case "ToDate":

                            var toDate = new DateTime();

                            if (DateTime.TryParse(filter.Value, out toDate))
                            {
                                switch (filter.Operator)
                                {
                                    case "eq":
                                        query = query.Where(x => x.ToDate == toDate);
                                        break;
                                    case "lt":
                                        query = query.Where(x => x.ToDate < toDate);
                                        break;
                                    case "le":
                                        query = query.Where(x => x.ToDate <= toDate);
                                        break;
                                    case "gt":
                                        query = query.Where(x => x.ToDate > toDate);
                                        break;
                                    case "ge":
                                        query = query.Where(x => x.ToDate >= toDate);
                                        break;
                                }
                            }

                            break;

                        case "Text":

                            switch (filter.Operator)
                            {
                                case "eq":
                                    query = query.Where(x => x.Text == filter.Value);
                                    break;
                                case "ct":
                                    query = query.Where(x => x.Text.Contains(filter.Value));
                                    break;
                            }

                            break;
                    }
                }
            }

            return query;
        }

        public static IQueryable<Note> Paginate(this IQueryable<Note> query, PageInfo pageInfo)
        {
            if (pageInfo != null)
            {
                switch (pageInfo.OrderBy)
                {
                    case "Name":
                        query = pageInfo.Desc.HasValue && pageInfo.Desc.Value 
                            ? query.OrderByDescending(x => x.Name) 
                            : query.OrderBy(x => x.Name);
                        break;
                    case "FromDate":
                        query = pageInfo.Desc.HasValue && pageInfo.Desc.Value 
                            ? query.OrderByDescending(x => x.FromDate) 
                            : query.OrderBy(x => x.FromDate);
                        break;
                    case "ToDate":
                        query = pageInfo.Desc.HasValue && pageInfo.Desc.Value 
                            ? query.OrderByDescending(x => x.ToDate) 
                            : query.OrderBy(x => x.ToDate);
                        break;
                    case "Text":
                        query = pageInfo.Desc.HasValue && pageInfo.Desc.Value 
                            ? query.OrderByDescending(x => x.Text) 
                            : query.OrderBy(x => x.Text);
                        break;
                    default:
                        query = pageInfo.Desc.HasValue && pageInfo.Desc.Value 
                            ? query.OrderByDescending(x => x.FromDate) 
                            : query.OrderBy(x => x.FromDate);
                        break;
                }

                query = query.Skip((pageInfo.PageIndex.Value - 1) * pageInfo.PageSize.Value).Take(pageInfo.PageSize.Value);
                
            }

            return query;
        }
    }
}
