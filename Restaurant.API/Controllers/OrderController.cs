﻿using AutoMapper;
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
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is unauthorized");
                _response.StatusCode = HttpStatusCode.Unauthorized;
                return Unauthorized(_response);
            }
            try
            {
                IEnumerable<OrderHeader> orders = await _unitOfWork.OrderHeader.GetAllAsync();

                orders = orders.OrderByDescending(item => item.Id);

                bool isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
                bool isStaff = await _userManager.IsInRoleAsync(user, SD.Role_Staff);

                if (isAdmin || isStaff)
                {
                    _response.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(orders);
                }
                else
                {
                    _response.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(orders.Where(u => u.ApplicationUserId == user.Id));
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
        [HttpGet("{id:int}", Name = "GetOrder")]
        public async Task<ActionResult<APIResponse>> GetOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);

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

                OrderHeader order = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == id);

                if (order == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Not found");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                bool isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
                bool isStaff = await _userManager.IsInRoleAsync(user, SD.Role_Staff);

                if (isAdmin || isStaff || order.ApplicationUserId == user.Id)
                {
                    order.OrderDetail = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderHeaderId == order.Id, includeProperties: "MenuItem");
                    _response.Result = _mapper.Map<OrderHeaderDTO>(order);
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
        public async Task<ActionResult<APIResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is unauthorized");
                _response.StatusCode = HttpStatusCode.Unauthorized;
                return Unauthorized(_response);
            }

            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCart.GetAsync(u => u.ApplicationUserId == user.Id);

            shoppingCart.CartItems = await _unitOfWork.CartItem.GetAllAsync(u => u.ShoppingCartId == shoppingCart.Id, includeProperties: "MenuItem");

            if (shoppingCart == null || shoppingCart.CartItems.Count() <= 0)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Shopping cart is empty");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }


            bool isTable = await _userManager.IsInRoleAsync(user, SD.Role_Table);
            double orderTotal = shoppingCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);
            double deliveryFee = isTable ? 0 : ((orderTotal > 100) ? 0 : 2);
            DeliveryInfo deliveryInfo = isTable ? null : orderHeaderDTO.DeliveryInfo;

            OrderHeader newOrderHeader = new OrderHeader()
            {
                ApplicationUserId = user.Id,
                ItemsTotal = shoppingCart.CartItems.Count(),
                OrderTotal = orderTotal,
                DeliveryFee = deliveryFee,
                OrderStatus = OrderStatus.Pending,
                DeliveryInfo = deliveryInfo,
            };

            await _unitOfWork.OrderHeader.CreateAsync(newOrderHeader);
            await _unitOfWork.SaveAsync();


            //create order details
            foreach (var cartItem in shoppingCart.CartItems)
            {
                var orderDetail = new OrderDetail()
                {
                    OrderHeaderId = newOrderHeader.Id,
                    MenuItemId = cartItem.MenuItem.Id,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.MenuItem.Price,
                    MenuItem = null
                };
                await _unitOfWork.OrderDetail.CreateAsync(orderDetail);
            }
            await _unitOfWork.SaveAsync();

            await _unitOfWork.ShoppingCart.RemoveAsync(shoppingCart);
            await _unitOfWork.SaveAsync();

            _response.Result = _mapper.Map<OrderHeaderDTO>(newOrderHeader);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetOrder", new { id = newOrderHeader.Id }, _response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateOrder(int id, [FromBody] OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {

            if (orderHeaderUpdateDTO == null || id != orderHeaderUpdateDTO.Id)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Id is not valid");
                return BadRequest(_response);
            }
            OrderHeader orderFromDb = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == id);
            if (orderFromDb == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Not Found");
                return NotFound(_response);
            }

            //update order
            var user = await _userManager.GetUserAsync(User);

            bool isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
            bool isStaff = await _userManager.IsInRoleAsync(user, SD.Role_Staff);
            bool isCustom = await _userManager.IsInRoleAsync(user, SD.Role_OnlCustomer);
            if (isAdmin || isStaff || isCustom)
            {
                if (isAdmin || isStaff)
                {
                    OrderStatus currentStatus = orderFromDb.OrderStatus;
                    if (!Enum.TryParse<OrderStatus>(orderHeaderUpdateDTO.OrderStatus, out var parsedStatus))
                    {
                        return BadRequest("Invalid order status.");
                    }
                    OrderStatus newStatus = parsedStatus;
                    if (IsValidTransition(currentStatus, newStatus))
                    {
                        orderFromDb.OrderStatus = newStatus;
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("Some thing went wrong");
                        return BadRequest(_response);
                    }

                }
                else
                {

                    if (orderFromDb.OrderStatus == OrderStatus.Cancelled || orderFromDb.OrderStatus == OrderStatus.Completed)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("You cannot update a completed or cancelled order.");
                        return BadRequest(_response);
                    }

                    if (orderHeaderUpdateDTO.DeliveryInfo != null)
                    {
                        orderFromDb.DeliveryInfo = orderHeaderUpdateDTO.DeliveryInfo;

                    }
                }

                await _unitOfWork.SaveAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("You do not have permission to update this order.");
                _response.StatusCode = HttpStatusCode.Unauthorized;
                return Unauthorized(_response);
            }
        }
        private bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    return newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Cancelled;

                case OrderStatus.Confirmed:
                    return newStatus == OrderStatus.BeingCooked || newStatus == OrderStatus.Cancelled;

                case OrderStatus.BeingCooked:
                    return newStatus == OrderStatus.ReadyForPickUp;

                case OrderStatus.ReadyForPickUp:
                    return newStatus == OrderStatus.Completed;

                case OrderStatus.Completed:
                    return false;
                case OrderStatus.Cancelled:
                    return false;
                default:
                    return false;
            }
        }
    }
}
