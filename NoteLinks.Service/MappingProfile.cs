using AutoMapper;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using NoteLinks.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Calendar, CalendarModel>().ReverseMap();
            CreateMap<CreateCalendarModel, Calendar>();
            CreateMap<Note, NoteModel>().ReverseMap();
            CreateMap<CreateNoteModel, Note>();
        }
    }
}
