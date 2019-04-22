using AutoMapper;
using NoteLinks.Data.Entities;
using NoteLinks.Service.ViewModels;

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
