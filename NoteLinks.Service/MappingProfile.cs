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
            CreateMap<PageInfoModel, PageInfo>()
                .ForMember(dest => dest.PageIndex, opt => opt.Condition(src => src.PageIndex > 0))
                .ForMember(dest => dest.PageSize, opt => opt.Condition(src => src.PageSize > 0));
        }
    }
}
