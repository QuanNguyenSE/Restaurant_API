using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartController(ApplicationDbContext db, IUnitOfWork unitOfWork)
        {
            _response = new APIResponse();
            _db = db;
            _unitOfWork = unitOfWork;

        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetShoppingCart(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                ShoppingCart shoppingCart = _db.ShoppingCarts
                    .Include(u => u.CartItems).ThenInclude(u => u.MenuItem)
                    .FirstOrDefault(u => u.UserId == userId);
                if (shoppingCart.CartItems != null && shoppingCart.CartItems.Count > 0)
                {
                    shoppingCart.CartTotal = shoppingCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);
                }
                _response.Result = shoppingCart;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
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
        public async Task<ActionResult<APIResponse>> AddOrUpdateItemInCart(string userId, int menuItemId, int updateQuantityBy)
        {
            // Shopping cart will have one entry per user id, even if a user has many items in cart.
            // Cart items will have all the items in shopping cart for a user
            // updatequantityby will have count by with an items quantity needs to be updated
            // if it is -1 that means we have lower a count if it is 5 it means we have to add 5 count to existing count.
            // if updatequantityby by is 0, item will be removed
            // when a user adds a new item to a new shopping cart for the first time
            // when a user adds a new item to an existing shopping cart (basically user has other items in cart)
            // when a user updates an existing item count
            // when a user removes an existing item

            ShoppingCart cart = _db.ShoppingCarts.Include(u => u.CartItems).FirstOrDefault(u => u.UserId == userId);
            MenuItem menuItem = await _unitOfWork.MenuItem.GetAsync(u => u.Id == menuItemId);
            if (menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Menu item is not valid");
                return BadRequest(_response);
            }
            if (cart == null && updateQuantityBy > 0)
            {
                //create a shopping cart & add cart item
                ShoppingCart newCart = new ShoppingCart()
                {
                    UserId = userId,
                    LastUpdated = DateTime.UtcNow,
                };
                _db.ShoppingCarts.Add(newCart);
                _db.SaveChanges();

                CartItem newCartItem = new CartItem()
                {
                    ShoppingCartId = newCart.Id,
                    Quantity = updateQuantityBy,
                    MenuItemId = menuItemId,
                    Price = menuItem.Price,
                    DateAdded = DateTime.UtcNow,
                    MenuItem = null
                };
                _db.CartItems.Add(newCartItem);
                _db.SaveChanges();
            }
            else
            {
                //shopping cart exists
                CartItem cartItem = cart.CartItems.FirstOrDefault(u => u.MenuItemId == menuItemId);
                if (cartItem == null)
                {
                    //item does not exists in cart
                    // add item into cart
                    CartItem newCartItem = new CartItem()
                    {
                        ShoppingCartId = cart.Id,
                        Quantity = updateQuantityBy,
                        MenuItemId = menuItemId,
                        Price = menuItem.Price,
                        DateAdded = DateTime.UtcNow,
                        MenuItem = null
                    };
                    _db.CartItems.Add(newCartItem);
                    _db.SaveChanges();
                }
                else
                {
                    //item exists in cart 
                    // update quantity
                    int newQuantity = cartItem.Quantity + updateQuantityBy;
                    if (newQuantity <= 0 || updateQuantityBy == 0)
                    {
                        _db.CartItems.Remove(cartItem);
                        _db.SaveChanges();
                        if (cart.CartItems.Count == 0)
                        {
                            _db.ShoppingCarts.Remove(cart);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        cartItem.Quantity = newQuantity;
                        _db.SaveChanges();
                    }

                }

            }
            return _response;
        }
    }
}
