using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.ExceptionHandling;
using NoteLinks.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteLinks.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ApiExceptionFilter))]
    public class NotesController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private INoteRepository _repository;
        private ILogger _logger;
        private IMapper _mapper;

        public NotesController(IUnitOfWork unitOfWork, ILogger<NotesController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Notes;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("{calendarCode}")]
        public async Task<IActionResult> Get(string calendarCode, [FromQuery] Filter[] filters, [FromQuery] PageInfo pageInfo)
        {
            if (String.IsNullOrEmpty(calendarCode))
                throw new ApiException("Calendar code is null.", StatusCodes.Status400BadRequest);

            filters = filters.Length == 0 ? null : filters;
            pageInfo = pageInfo.PageIndex is null ? null : pageInfo;

            var totalCount = await _repository.GetNotesCountAsync(x => x.Calendar.Code == calendarCode, filters);
            List<Note> list = await _repository.GetNotesAsync(x => x.Calendar.Code == calendarCode, filters, pageInfo);

            return Ok(new NoteResultModel()
            {                    
                Notes = _mapper.Map<List<Note>, List<NoteModel>>(list),
                TotalCount = totalCount
            });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]        
        public async Task<IActionResult> Post([FromBody] CreateNoteModel model)
        {
            if (model is null)
                throw new ApiException("Model is null.", StatusCodes.Status400BadRequest);

            var calendar = await _unitOfWork.Calendars.SingleOrDefaultAsync(x => x.Code == model.CalendarCode);

            if (calendar is null)          
                throw new ApiException("Calendar doesn't exist.", StatusCodes.Status404NotFound);            

            var entity = _mapper.Map<CreateNoteModel, Note>(model);
            entity.CalendarId = calendar.Id;

            _repository.Add(entity);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<Note, NoteModel>(entity));
        }

        [HttpPut]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Put([FromBody] NoteModel model)
        {
            if (model is null)
                throw new ApiException("Model is null.", StatusCodes.Status400BadRequest);

            var entity = await _repository.GetAsync(model.Id);

            if (entity is null)
                throw new ApiException("Note doesn't exist.", StatusCodes.Status404NotFound);

            _mapper.Map(model, entity);

            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<Note, NoteModel>(entity));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetAsync(id);

            if (entity is null)
                throw new ApiException("Note doesn't exist.", StatusCodes.Status404NotFound);

            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }
    }
}
