using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Repository.IRepository;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ShoppingCartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _response = new APIResponse();
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetShoppingCart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _response.Result = null;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("User is not valid");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                ShoppingCart shoppingCart = await _unitOfWork.ShoppingCart.GetAsync(u => u.ApplicationUserId == user.Id);
                if (shoppingCart == null)
                {
                    // shopping cart doees not exists
                    // create a shopping cart
                    shoppingCart = new ShoppingCart
                    {
                        ApplicationUserId = user.Id,
                        CartTotal = 0,
                        ItemsTotal = 0,
                        LastUpdated = DateTime.UtcNow,
                        CartItems = new List<CartItem>()
                    };
                    await _unitOfWork.ShoppingCart.CreateAsync(shoppingCart);
                    await _unitOfWork.SaveAsync();
                    _response.Result = _mapper.Map<ShoppingCartDTO>(shoppingCart);
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);

                }
                else
                {
                    shoppingCart.CartItems = await _unitOfWork.CartItem.GetAllAsync(u => u.ShoppingCartId == shoppingCart.Id, includeProperties: "MenuItem");
                    if (shoppingCart.CartItems != null && shoppingCart.CartItems.Count() > 0)
                    {
                        await _unitOfWork.ShoppingCart.UpdateAsync(shoppingCart);
                        await _unitOfWork.SaveAsync();
                    }
                    _response.Result = _mapper.Map<ShoppingCartDTO>(shoppingCart);
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> AddOrUpdateItemInCart(int menuItemId, int updateQuantity)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _response.Result = null;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is not valid");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            ShoppingCart cart = await _unitOfWork.ShoppingCart.GetAsync(u => u.ApplicationUserId == user.Id);
            MenuItem menuItem = await _unitOfWork.MenuItem.GetAsync(u => u.Id == menuItemId);
            if (menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Menu item is not valid");
                return BadRequest(_response);
            }
            if (cart == null && updateQuantity > 0)
            {
                // shopping cart doees not exists
                //create a shopping cart 
                ShoppingCart newCart = new ShoppingCart()
                {
                    ApplicationUserId = user.Id,
                    CartTotal = 0,
                    ItemsTotal = 0,
                    LastUpdated = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                await _unitOfWork.ShoppingCart.CreateAsync(newCart);
                await _unitOfWork.SaveAsync();
                // add cart item
                CartItem newCartItem = new CartItem()
                {
                    ShoppingCartId = newCart.Id,
                    Quantity = updateQuantity,
                    MenuItemId = menuItemId,
                    DateAdded = DateTime.UtcNow,
                    MenuItem = menuItem
                };
                await _unitOfWork.CartItem.CreateAsync(newCartItem);
                await _unitOfWork.SaveAsync();

                newCart.CartItems = await _unitOfWork.CartItem.GetAllAsync(u => u.ShoppingCartId == newCart.Id, includeProperties: "MenuItem");
                await _unitOfWork.ShoppingCart.UpdateAsync(newCart);
                await _unitOfWork.SaveAsync();


                _response.Result = _mapper.Map<ShoppingCartDTO>(newCart);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            else
            {
                //shopping cart exists
                CartItem cartItem = await _unitOfWork.CartItem.GetAsync(u => u.ShoppingCartId == cart.Id && u.MenuItemId == menuItem.Id);
                if (cartItem == null && updateQuantity > 0)
                {
                    //item does not exists in cart
                    // add item into cart
                    CartItem newCartItem = new CartItem()
                    {
                        ShoppingCartId = cart.Id,
                        Quantity = updateQuantity,
                        MenuItemId = menuItemId,
                        DateAdded = DateTime.UtcNow,
                        MenuItem = menuItem
                    };
                    await _unitOfWork.CartItem.CreateAsync(newCartItem);
                    await _unitOfWork.SaveAsync();
                    cart.CartItems = await _unitOfWork.CartItem.GetAllAsync(u => u.ShoppingCartId == cart.Id, includeProperties: "MenuItem");
                    await _unitOfWork.ShoppingCart.UpdateAsync(cart);
                    await _unitOfWork.SaveAsync();

                    _response.Result = _mapper.Map<ShoppingCartDTO>(cart);
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
                else
                {
                    //item exists in cart 
                    // update quantity (minus or add)
                    int newQuantity = cartItem.Quantity + updateQuantity;
                    if (newQuantity <= 0)
                    {
                        await _unitOfWork.CartItem.RemoveAsync(cartItem);
                        await _unitOfWork.SaveAsync();
                        cart.CartItems = await _unitOfWork.CartItem.GetAllAsync(u => u.ShoppingCartId == cart.Id, includeProperties: "MenuItem");
                        await _unitOfWork.ShoppingCart.UpdateAsync(cart);
                        await _unitOfWork.SaveAsync();


                        _response.Result = _mapper.Map<ShoppingCartDTO>(cart);
                        _response.StatusCode = HttpStatusCode.OK;
                        return Ok(_response);
                    }
                    else
                    {
                        cartItem.Quantity = newQuantity;
                        await _unitOfWork.SaveAsync();
                        cart.CartItems = await _unitOfWork.CartItem.GetAllAsync(u => u.ShoppingCartId == cart.Id, includeProperties: "MenuItem");
                        await _unitOfWork.ShoppingCart.UpdateAsync(cart);
                        await _unitOfWork.SaveAsync();


                        _response.Result = _mapper.Map<ShoppingCartDTO>(cart);
                        _response.StatusCode = HttpStatusCode.OK;
                        return Ok(_response);
                    }

                }

            }


        }
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> RemoveItemInCart(int cartItemId)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _response.Result = null;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is not valid");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            ShoppingCart cart = await _unitOfWork.ShoppingCart.GetAsync(u => u.ApplicationUserId == user.Id);
            CartItem cartItem = await _unitOfWork.CartItem.GetAsync(u => u.Id == cartItemId);
            if (cartItem == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Cart item does not exists");
                return BadRequest(_response);
            }
            else
            {
                await _unitOfWork.CartItem.RemoveAsync(cartItem);
                await _unitOfWork.SaveAsync();
                cart.CartItems = await _unitOfWork.CartItem.GetAllAsync(u => u.ShoppingCartId == cart.Id, includeProperties: "MenuItem");
                await _unitOfWork.ShoppingCart.UpdateAsync(cart);
                await _unitOfWork.SaveAsync();


                _response.Result = _mapper.Map<ShoppingCartDTO>(cart);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }
    }
}
