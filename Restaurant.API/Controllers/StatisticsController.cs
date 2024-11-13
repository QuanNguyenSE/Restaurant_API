using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;
using Restaurant.API.Utility;
using System.Net;

namespace Restaurant.API.Controllers
{
    //[Authorize(Roles = SD.Role_Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly APIResponse _response;

        public StatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
        }
        [HttpGet("revenue")]
        public async Task<ActionResult<APIResponse>> GetRevenue([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, OrderStatus status = OrderStatus.Completed)
        {
            try
            {
                var orders = await _unitOfWork.OrderHeader.GetAllAsync(u => u.OrderStatus == status, tracking: false);
                if (startDate != null)
                {
                    orders = orders.Where(u => u.OrderDate >= startDate);
                }
                if (endDate != null)
                {
                    orders = orders.Where(u => u.OrderDate <= endDate);
                }

                var totalRevenue = orders.Sum(o => o.OrderTotal);

                _response.Result = totalRevenue;
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

        [HttpGet("revenue-by-period")]
        public async Task<ActionResult<APIResponse>> GetRevenueByPeriod([FromQuery] string period, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var orders = await _unitOfWork.OrderHeader.GetAllAsync(u => u.OrderStatus == OrderStatus.Completed, tracking: false);
                if (startDate != null)
                {
                    orders = orders.Where(u => u.OrderDate >= startDate);
                }
                if (endDate != null)
                {
                    orders = orders.Where(u => u.OrderDate <= endDate);
                }
                var revenue = orders.GroupBy(o => period.ToLower() switch
                   {
                       "day" => o.OrderDate.Date,
                       "month" => new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1),
                       "year" => new DateTime(o.OrderDate.Year, 1, 1),
                       _ => o.OrderDate.Date
                   })
                   .Select(g => new
                   {
                       Period = g.Key,
                       TotalRevenue = g.Sum(o => o.OrderTotal)
                   })
                   .OrderBy(r => r.Period)
                   .ToList();


                _response.Result = revenue;
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

        [HttpGet("revenue-by-menuitem")]
        public async Task<ActionResult<APIResponse>> GetRevenueByMenuItem([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var orderItems = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderHeader.OrderStatus == OrderStatus.Completed, tracking: false, includeProperties: "MenuItem,OrderHeader");

                if (startDate != null)
                {
                    orderItems = orderItems.Where(u => u.OrderHeader.OrderDate >= startDate);
                }
                if (endDate != null)
                {
                    orderItems = orderItems.Where(u => u.OrderHeader.OrderDate <= endDate);
                }
                var revenue = orderItems
                    .GroupBy(u => u.MenuItem.Id)
                    .Select(g => new
                    {
                        MenuItemId = g.Key,
                        MenuItem = g.First().MenuItem.Name,
                        TotalRevenue = g.Sum(u => u.Price * u.Quantity),
                        TotalQuantity = g.Sum(u => u.Quantity)
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToList();

                _response.Result = revenue;
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

        [HttpGet("revenue-by-category")]
        public async Task<IActionResult> GetRevenueByCategory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var orderItems = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderHeader.OrderStatus == OrderStatus.Completed, tracking: false, includeProperties: "MenuItem.Category,OrderHeader");

                if (startDate != null)
                {
                    orderItems = orderItems.Where(u => u.OrderHeader.OrderDate >= startDate);
                }
                if (endDate != null)
                {
                    orderItems = orderItems.Where(u => u.OrderHeader.OrderDate <= endDate);
                }
                var revenue = orderItems
                    .GroupBy(u => u.MenuItem.CategoryId)
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        Category = g.First().MenuItem.Category.Name,
                        TotalRevenue = g.Sum(u => u.Price * u.Quantity),
                        TotalQuantity = g.Sum(u => u.Quantity)
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToList();

                _response.Result = revenue;
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


        [HttpGet("customer-count")]
        public async Task<ActionResult<APIResponse>> GetCustomerCount([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, OrderStatus status = OrderStatus.Completed)
        {

            try
            {
                var orders = await _unitOfWork.OrderHeader.GetAllAsync(u => u.OrderStatus == status, tracking: false);
                if (startDate != null)
                {
                    orders = orders.Where(u => u.OrderDate >= startDate);
                }
                if (endDate != null)
                {
                    orders = orders.Where(u => u.OrderDate <= endDate);
                }
                var customerCount = orders
                    .Select(u => u.ApplicationUserId)
                    .Distinct()
                    .Count();
                _response.Result = customerCount;
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

        [HttpGet("booking-count")]
        public async Task<ActionResult<APIResponse>> GetBookingCount([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, BookingStatus status = BookingStatus.Completed)
        {
            try
            {
                var books = await _unitOfWork.Booking.GetAllAsync(u => u.BookingStatus == status, tracking: false);
                if (startDate != null)
                {
                    books = books.Where(u => u.BookingDate >= startDate);
                }
                if (endDate != null)
                {
                    books = books.Where(u => u.BookingDate <= endDate);
                }

                var totalbooking = books.Count();

                _response.Result = totalbooking;
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

    }
}
