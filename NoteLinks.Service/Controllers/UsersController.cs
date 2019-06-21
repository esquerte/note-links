using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.Authentication;
using NoteLinks.Service.ExceptionHandling;
using NoteLinks.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoteLinks.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ApiExceptionFilter))]
    public class UsersController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<User> _userManager;
        private IUserRepository _repository;
        private ILogger<UsersController> _logger;
        private IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<UsersController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Users;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // TODO: Email verification
        // TODO: User update and password changing

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AuthenticationModel model)
        {
            User user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new ApiException("The email or password is incorrect.", StatusCodes.Status401Unauthorized);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.Now.AddMinutes(AuthOptions.LIFETIME),
            signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            var result = new AuthenticationResultModel
            {
                AccessToken = encodedJwt,
                DisplayName = user.DisplayName,
                ExpirationTime = AuthOptions.LIFETIME
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserModel model)
        {
            var user = new User { Email = model.Email, UserName = model.Email, DisplayName = model.DisplayName };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);

                throw new ApiException("Incorrect registration data.", StatusCodes.Status400BadRequest);
            }

            return Ok();
        }

        [HttpPost]
        [Route("delete")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] DeleteUserModel model)
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new ApiException("Unauthorized access.", StatusCodes.Status401Unauthorized);

            await _repository.DeleteUserDataAsync(user.Id, model.DeleteCalendars);

            IdentityResult result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);

                throw new ApiException("Error occurred while deleting user.");
            }

            return Ok();
        }

        [HttpGet]
        [Route("calendars")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            // Claim with type ClaimTypes.NameIdentifier should exists in claims,
            // otherwise GetUserAsync will return null
            User user = await _userManager.GetUserAsync(HttpContext.User);

            if (user is null)
                throw new ApiException("Unauthorized access.", StatusCodes.Status401Unauthorized);

            List<Calendar> list = await _repository.GetUserCalendarsAsync(user.Id);

            var result = new List<UserCalendarModel>();

            foreach (var calendar in list)
            {
                var userCalendar = new UserCalendarModel
                {
                    Code = calendar.Code,
                    Name = calendar.Name,
                    Creator = calendar.Creator != null && calendar.Creator.Id == user.Id
                };

                result.Add(userCalendar);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("calendars")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateUserCalendarModel model)
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            if (user is null)
                throw new ApiException("Unauthorized access.", StatusCodes.Status401Unauthorized);

            var calendar = await _unitOfWork.Calendars.SingleOrDefaultAsync(x => x.Code == model.Code);

            if (calendar is null)
                throw new ApiException("Calendar doesn't exist.", StatusCodes.Status404NotFound);

            await _repository.AddUserCalendarAsync(user.Id, calendar.Id);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("calendars/{code}")]        
        [Authorize]
        public async Task<IActionResult> Delete(string code)
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            if (user is null)
                throw new ApiException("Unauthorized access.", StatusCodes.Status401Unauthorized);

            var calendar = await _unitOfWork.Calendars.SingleOrDefaultAsync(x => x.Code == code);

            if (calendar is null)
                throw new ApiException("Calendar doesn't exist.", StatusCodes.Status404NotFound);

            if (calendar.Creator != null)
            {
                if (calendar.Creator.Id == user.Id)
                    throw new ApiException("The calendar can't be unfollowed by its creator.", StatusCodes.Status400BadRequest);
            }

            await _repository.DeleteUserCalendarAsync(user.Id, calendar.Id);

            return Ok();
        }
    }
}
