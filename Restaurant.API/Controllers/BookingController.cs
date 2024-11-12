using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Repository.IRepository;
using Restaurant.API.Utility;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetBookings(string userId)
        {
            //var user = await _userManager.GetUserAsync(User);
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is unauthorized");
                _response.StatusCode = HttpStatusCode.Unauthorized;
                return Unauthorized(_response);
            }
            try
            {
                IEnumerable<Booking> bookings = await _unitOfWork.Booking.GetAllAsync();

                bookings = bookings.OrderByDescending(item => item.Id);

                bool isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
                bool isStaff = await _userManager.IsInRoleAsync(user, SD.Role_Staff);

                if (isAdmin || isStaff)
                {
                    _response.Result = _mapper.Map<IEnumerable<BookingDTO>>(bookings);
                }
                else
                {
                    _response.Result = _mapper.Map<IEnumerable<BookingDTO>>(bookings.Where(u => u.ApplicationUserId == user.Id));
                }
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        [HttpGet("{id:int}", Name = "GetBooking")]
        public async Task<ActionResult<APIResponse>> GetBooking(int id, string userId)
        {
            //var user = await _userManager.GetUserAsync(User);
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is unauthorized");
                _response.StatusCode = HttpStatusCode.Unauthorized;
                return Unauthorized(_response);
            }
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Id is not valid");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Booking booking = await _unitOfWork.Booking.GetAsync(u => u.Id == id);

                if (booking == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Not found");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                bool isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
                bool isStaff = await _userManager.IsInRoleAsync(user, SD.Role_Staff);

                if (isAdmin || isStaff || booking.ApplicationUserId == user.Id)
                {
                    _response.Result = _mapper.Map<BookingDTO>(booking);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("User is unauthorized");
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return _response;
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateBooking([FromForm] BookingCreateDTO bookingDTO)
        {
            //var user = await _userManager.GetUserAsync(User);
            ApplicationUser user = await _userManager.FindByIdAsync(bookingDTO.ApplicationUserId);
            if (user == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is unauthorized");
                _response.StatusCode = HttpStatusCode.Unauthorized;
                return Unauthorized(_response);
            }

            Booking newBooking = _mapper.Map<Booking>(bookingDTO);

            await _unitOfWork.Booking.CreateAsync(newBooking);
            await _unitOfWork.SaveAsync();

            _response.Result = _mapper.Map<BookingDTO>(newBooking);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetBooking", new { id = newBooking.Id }, _response);
        }
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult<APIResponse>> UpdateBooking(string userId, int id, [FromBody] BookingUpdateDTO bookingDTO)
        //{
        //    //var user = await _userManager.GetUserAsync(User);
        //    ApplicationUser user = await _userManager.FindByIdAsync(userId);

        //    if (user == null)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages.Add("User is unauthorized");
        //        _response.StatusCode = HttpStatusCode.Unauthorized;
        //        return Unauthorized(_response);
        //    }
        //    if (id == 0 || id != bookingDTO.Id)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages.Add("Id is not valid");
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        return BadRequest(_response);
        //    }

        //    Booking booking = await _unitOfWork.Booking.GetAsync(u => u.Id == id);

        //    if (booking == null)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages.Add("Not found");
        //        _response.StatusCode = HttpStatusCode.NotFound;
        //        return NotFound(_response);
        //    }

        //    bool isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
        //    bool isStaff = await _userManager.IsInRoleAsync(user, SD.Role_Staff);

        //    if (isAdmin || isStaff)
        //    {
        //        _response.Result = _mapper.Map<BookingDTO>(booking);
        //    }
        //    else
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages.Add("User is unauthorized");
        //        _response.StatusCode = HttpStatusCode.Unauthorized;
        //        return Unauthorized(_response);
        //    }

        //    _response.StatusCode = HttpStatusCode.OK;
        //    return Ok(_response);
        //}
    }
}
