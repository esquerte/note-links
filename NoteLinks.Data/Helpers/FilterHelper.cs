using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Helpers
{
    public static class FilterHelper
    {
        public static IQueryable<Note> FilterNotes(ref IQueryable<Note> query, Filter[] filters)
        {
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    switch (filter.Field)
                    {
                        case "Name":

                            switch (filter.Operator)
                            {
                                case "eq":
                                    query = query.Where(x => x.Name == filter.Value);
                                    break;
                                case "cts":
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
                                    case "lte":
                                        query = query.Where(x => x.FromDate <= fromDate);
                                        break;
                                    case "gt":
                                        query = query.Where(x => x.FromDate > fromDate);
                                        break;
                                    case "gte":
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
                                    case "lte":
                                        query = query.Where(x => x.ToDate <= toDate);
                                        break;
                                    case "gt":
                                        query = query.Where(x => x.ToDate > toDate);
                                        break;
                                    case "gte":
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
                                case "cts":
                                    query = query.Where(x => x.Text.Contains(filter.Value));
                                    break;
                            }

                            break;
                    }
                }
            }

            return query;
        }
    }
}
