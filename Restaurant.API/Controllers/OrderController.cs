﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Utility;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected APIResponse _response;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
            _response = new APIResponse();
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetOrders(string? userId)
        {
            try
            {
                var orders = _db.OrderHeaders.Include(u => u.OrderDetail)
                .ThenInclude(u => u.MenuItem)
                .OrderByDescending(u => u.Id);

                if (!string.IsNullOrEmpty(userId))
                {
                    _response.Result = orders.Where(u => u.ApplicationUserId == userId);
                }
                else
                {
                    _response.Result = orders;
                }
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
            }
            return _response;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<APIResponse>> GetOrder(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(_response);
                }

                var orders = _db.OrderHeaders.Include(u => u.OrderDetail)
                .ThenInclude(u => u.MenuItem)
                .Where(u => u.Id == id);

                if (orders == null)
                {
                    return NotFound(_response);
                }

                _response.Result = orders;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
            }
            return _response;
        }
        [HttpPost]

        public async Task<ActionResult<APIResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            OrderHeader newOrderHeader = new OrderHeader()
            {
                ApplicationUserId = orderHeaderDTO.ApplicationUserId,
                OrderDate = DateTime.Now,
                OrderTotal = orderHeaderDTO.OrderTotal,
                ItemsTotal = orderHeaderDTO.ItemsTotal,
                OrderStatus = String.IsNullOrEmpty(orderHeaderDTO.OrderStatus) ? SD.StatusPending : orderHeaderDTO.OrderStatus,
                PaymentIntentId = orderHeaderDTO.PaymentIntentId,
                Email = orderHeaderDTO.Email,
                Name = orderHeaderDTO.Name,
                PhoneNumber = orderHeaderDTO.PhoneNumber,
                StreetAddress = orderHeaderDTO.StreetAddress,
                City = orderHeaderDTO.City,
                State = orderHeaderDTO.State,
            };
            if (ModelState.IsValid)
            {
                _db.OrderHeaders.Add(newOrderHeader);
                _db.SaveChanges();
                foreach (OrderDetailsCreateDTO orderDetailDTO in orderHeaderDTO.OrderDetail)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = newOrderHeader.Id,
                        MenuItemId = orderDetailDTO.MenuItemId,
                        Quantity = orderDetailDTO.Quantity,
                        Price = orderDetailDTO.Price,
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();
            }
            return _response;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateOrderHeader(int id, [FromBody] OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            try
            {
                if (orderHeaderUpdateDTO == null || id != orderHeaderUpdateDTO.Id)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Id is not valid");
                    return BadRequest(_response);
                }
                OrderHeader orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);

                if (orderFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Not Found");

                    return NotFound(_response);
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.Name))
                {
                    orderFromDb.Name = orderHeaderUpdateDTO.Name;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PhoneNumber))
                {
                    orderFromDb.PhoneNumber = orderHeaderUpdateDTO.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.Email))
                {
                    orderFromDb.Email = orderHeaderUpdateDTO.Email;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.OrderStatus))
                {
                    orderFromDb.OrderStatus = orderHeaderUpdateDTO.OrderStatus;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PaymentIntentId))
                {
                    orderFromDb.PaymentIntentId = orderHeaderUpdateDTO.PaymentIntentId;
                }
                _db.SaveChanges();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
