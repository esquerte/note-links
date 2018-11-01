using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.Logging;
using NoteLinks.Service.ViewModels;

namespace NoteLinks.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private INoteRepository _repository;
        private ILogger _logger;

        public NotesController(IUnitOfWork unitOfWork, ILogger<CalendarsController> logger)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Notes;
            _logger = logger;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            try
            {
                if (String.IsNullOrEmpty(code)) {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, $"Get({code}) NOT FOUND");
                    return NotFound();
                }

                var list = await _repository.FindAsync(x => x.Calendar.Code == code);
                return new ObjectResult(list.Select(x => new NoteModel() {
                    Id = x.Id,
                    Name = x.Name,
                    Text = x.Text,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate
                }));

            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.GetItemError, exception, $"Get({code})");
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
                    return BadRequest();
                }

                var entity = new Note() {
                    Name = model.Name,
                    Text = model.Text,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CalendarId = calendar.Id
                };

                _repository.Add(entity);
                await _unitOfWork.CompleteAsync();

                return Ok(new NoteModel()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Text = entity.Text,
                    FromDate = entity.FromDate,
                    ToDate = entity.ToDate
                });
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
                var note = await _repository.GetAsync(model.Id);

                if (note is null)
                {
                    _logger.LogWarning(LoggingEvents.UpdateItemNotFound, $"Put({JsonConvert.SerializeObject(model)}) NOT FOUND");
                    return NotFound();
                }

                note.Name = model.Name;
                note.FromDate = model.FromDate;
                note.ToDate = model.ToDate;
                note.Text = model.Text;

                _repository.Update(note);
                await _unitOfWork.CompleteAsync();

                return Ok();
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
