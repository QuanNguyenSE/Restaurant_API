using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Repository.IRepository;
using Restaurant.API.Utility;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
            _userManager = userManager;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetBookings()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
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
                _response.ErrorMessages.Add(ex.Message);
                return BadRequest(_response);
            }
        }
        [HttpGet("{id:int}", Name = "GetBooking")]
        public async Task<ActionResult<APIResponse>> GetBooking(int id)
        {
            try
            {
                if (id == 0)
                {
                    throw new Exception("Id is not valid");
                }
                Booking booking = await _unitOfWork.Booking.GetAsync(u => u.Id == id);
                if (booking == null)
                {
                    throw new Exception("Not found");
                }
                var user = await _userManager.GetUserAsync(User);
                bool isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
                bool isStaff = await _userManager.IsInRoleAsync(user, SD.Role_Staff);

                if (isAdmin || isStaff || booking.ApplicationUserId == user.Id)
                {
                    _response.Result = _mapper.Map<BookingDTO>(booking);
                }
                else
                {
                    throw new Exception("User is unauthorized");
                }
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message);
                return BadRequest(_response);
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateBooking([FromForm] BookingCreateDTO bookingDTO)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                Booking newBooking = _mapper.Map<Booking>(bookingDTO);
                newBooking.ApplicationUserId = user.Id;
                await _unitOfWork.Booking.CreateAsync(newBooking);
                await _unitOfWork.SaveAsync();

                _response.Result = _mapper.Map<BookingDTO>(newBooking);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetBooking", new { id = newBooking.Id }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Staff)]
        public async Task<ActionResult<APIResponse>> UpdateBooking(int id, [FromBody] BookingUpdateDTO bookingDTO)
        {
            try
            {
                if (id == 0 || id != bookingDTO.Id)
                {
                    throw new Exception("Id is not valid");
                }
                Booking booking = await _unitOfWork.Booking.GetAsync(u => u.Id == id, tracking: false);
                if (booking == null)
                {
                    throw new Exception("Not found");
                }

                _mapper.Map(bookingDTO, booking);
                await _unitOfWork.Booking.UpdateAsync(booking);
                await _unitOfWork.SaveAsync();

                if (bookingDTO.BookingStatus != null)
                {
                    if (!Enum.TryParse<BookingStatus>(bookingDTO.BookingStatus, out var parsedStatus))
                    {
                        throw new Exception("Booking status is not valid");
                    }

                    // update status
                    BookingStatus currentStatus = booking.BookingStatus;
                    BookingStatus newStatus = parsedStatus;

                    if (IsValidTransition(currentStatus, newStatus))
                    {
                        booking.BookingStatus = newStatus;
                        await _unitOfWork.Booking.UpdateAsync(booking);
                        await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        throw new Exception("Booking status is not valid");
                    }
                }
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        private bool IsValidTransition(BookingStatus currentStatus, BookingStatus newStatus)
        {
            return currentStatus switch
            {
                BookingStatus.Pending => newStatus == BookingStatus.Confirmed || newStatus == BookingStatus.Cancelled,
                BookingStatus.Confirmed => newStatus == BookingStatus.CheckedIn || newStatus == BookingStatus.Cancelled,
                BookingStatus.CheckedIn => newStatus == BookingStatus.Occupied || newStatus == BookingStatus.Cancelled,
                BookingStatus.Occupied => newStatus == BookingStatus.Completed,
                BookingStatus.Completed => false, // Cannot transition from Completed
                BookingStatus.Cancelled => false, // Cannot transition from Canceled
                _ => false,
            };
        }
    }
}
