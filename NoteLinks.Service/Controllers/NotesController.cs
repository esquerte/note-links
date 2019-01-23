using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Data.Models;
using NoteLinks.Service.Logging;
using NoteLinks.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace NoteLinks.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            try
            {
                if (String.IsNullOrEmpty(calendarCode)) {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, $"Get({calendarCode}) NOT FOUND");
                    return NotFound();
                }

                var totalCount = await _repository.GetNotesCountAsync(x => x.Calendar.Code == calendarCode, filters);

                var list = await _repository.GetNotesAsync(x => x.Calendar.Code == calendarCode, filters, pageInfo);

                return new ObjectResult(new ResultNoteModel()
                {                    
                    Notes = _mapper.Map<List<Note>, List<NoteModel>>(list),
                    TotalCount = totalCount
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.GetItemError, exception, $"Get({calendarCode})");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateNoteModel model)
        {
            if (model is null)
                return BadRequest();

            try
            {
                var calendar = await _unitOfWork.Calendars.SingleOrDefaultAsync(x => x.Code == model.CalendarCode);

                if (calendar is null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, $"Post({JsonConvert.SerializeObject(model)}) NOT FOUND");
                    return NotFound();
                }

                var entity = _mapper.Map<CreateNoteModel, Note>(model);
                entity.CalendarId = calendar.Id;

                _repository.Add(entity);
                await _unitOfWork.CompleteAsync();

                return Ok(_mapper.Map<Note, NoteModel>(entity));
            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.InsertItemError, exception, $"Post({JsonConvert.SerializeObject(model)})");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] NoteModel model)
        {
            if (model is null)
                return BadRequest();

            try
            {
                var entity = await _repository.GetAsync(model.Id);

                if (entity is null)
                {
                    _logger.LogWarning(LoggingEvents.UpdateItemNotFound, $"Put({JsonConvert.SerializeObject(model)}) NOT FOUND");
                    return NotFound();
                }

                _mapper.Map(model, entity);

                _repository.Update(entity);
                await _unitOfWork.CompleteAsync();

                return Ok(_mapper.Map<Note, NoteModel>(entity));
            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.UpdateItemError, exception, $"Put({JsonConvert.SerializeObject(model)})");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _repository.GetAsync(id);

                if (entity is null)
                {
                    _logger.LogWarning(LoggingEvents.DeleteItemNotFound, $"Delete({id}) NOT FOUND");
                    return NotFound();
                }

                _repository.Remove(entity);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.DeleteItemError, exception, $"Delete({id})");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
