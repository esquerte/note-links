using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.Helpers;
using NoteLinks.Service.Logging;
using NoteLinks.Service.ViewModels;
using System;
using System.Threading.Tasks;

namespace NoteLinks.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarsController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private ICalendarRepository _repository;
        private ILogger _logger;

        public CalendarsController(IUnitOfWork unitOfWork, ILogger<CalendarsController> logger)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Calendars;
            _logger = logger;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            try
            {                
                var entity = await _repository.SingleOrDefaultAsync(x => x.Code == code);

                if (entity is null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, $"Get({code}) NOT FOUND");
                    return NotFound();
                }

                return new ObjectResult(new CalendarModel(entity));
            }
            catch(Exception exception)
            {
                _logger.LogError(LoggingEvents.GetItemError, exception, $"Get({code})");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCalendarModel model)
        {
            if (model is null)
                return BadRequest();

            try
            {
                var entity = new Calendar() {
                    Code = CodeHelper.GetCode(),
                    Name = model.Name
                };

                _repository.Add(entity);
                await _unitOfWork.CompleteAsync();

                return Ok(new CalendarModel(entity));
            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.InsertItemError, exception, $"Post({JsonConvert.SerializeObject(model)})");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CalendarModel model)
        {
            if (model is null)
                return BadRequest();

            try
            {
                var calendar = await _repository.SingleOrDefaultAsync(x => x.Code == model.Code);

                if (calendar is null)
                {
                    _logger.LogWarning(LoggingEvents.UpdateItemNotFound, $"Put({JsonConvert.SerializeObject(model)}) NOT FOUND");
                    return NotFound();
                }

                calendar.Name = model.Name;

                _repository.Update(calendar);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.UpdateItemError, exception, $"Put({JsonConvert.SerializeObject(model)})");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                var entity = await _repository.SingleOrDefaultAsync(x => x.Code == code);

                if (entity is null)
                {
                    _logger.LogWarning(LoggingEvents.DeleteItemNotFound, $"Delete({code}) NOT FOUND");
                    return NotFound();
                }

                _repository.Remove(entity);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(LoggingEvents.DeleteItemError, exception, $"Delete({code})");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
