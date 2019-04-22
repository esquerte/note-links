using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.ExceptionHandling;
using NoteLinks.Service.Helpers;
using NoteLinks.Service.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteLinks.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ApiExceptionFilter))]
    public class CalendarsController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<User> _userManager;
        private ICalendarRepository _repository;
        private ILogger _logger;
        private IMapper _mapper;

        public CalendarsController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<CalendarsController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Calendars;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            var entity = await _repository.SingleOrDefaultAsync(x => x.Code == code);

            if (entity is null)
                throw new ApiException("Calendar doesn't exist.", StatusCodes.Status404NotFound);

            return Ok(_mapper.Map<Calendar, CalendarModel>(entity));
        }    

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCalendarModel model)
        {
            if (model is null)
                throw new ApiException("Model is null.", StatusCodes.Status400BadRequest);

            var entity = _mapper.Map<CreateCalendarModel, Calendar>(model);
            entity.Code = CodeHelper.GetCode();

            User user = await _userManager.GetUserAsync(HttpContext.User);
            entity.Creator = user;

            _repository.Add(entity);
            await _unitOfWork.CompleteAsync();

            if (user != null)
                await _unitOfWork.Users.AddUserCalendarAsync(user.Id, entity.Id);

            return Ok(_mapper.Map<Calendar, CalendarModel>(entity));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CalendarModel model)
        {
            if (model is null)
                throw new ApiException("Model is null.", StatusCodes.Status400BadRequest);

            var entity = await _repository.SingleOrDefaultAsync(x => x.Code == model.Code);

            if (entity is null)
                throw new ApiException("Calendar doesn't exist.", StatusCodes.Status404NotFound);

            _mapper.Map(model, entity);

            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<Calendar, CalendarModel>(entity));
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var entity = await _repository.GetCalendarWithCreator(x => x.Code == code);            

            if (entity is null)
                throw new ApiException("Calendar doesn't exist.", StatusCodes.Status404NotFound);

            if (entity.Creator != null)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (user is null || entity.Creator.Id != user.Id)
                    throw new ApiException("The calendar can only be deleted by its creator.", StatusCodes.Status401Unauthorized);
            }

            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }
    }
}
