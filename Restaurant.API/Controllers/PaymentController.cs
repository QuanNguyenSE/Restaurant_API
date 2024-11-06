using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Stripe;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IConfiguration _congifuration;
        private readonly ApplicationDbContext _db;
        public PaymentController(IConfiguration configuration, ApplicationDbContext db)
        {
            _congifuration = configuration;
            _db = db;
            _response = new();
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> MakePayment(string userId)
        {
            ShoppingCart shoppingCart = _db.ShoppingCarts.Include(u => u.CartItems).ThenInclude(u => u.MenuItem).FirstOrDefault(u => u.ApplicationUserId == userId);
            if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count() == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            #region Create Payment Intent
            StripeConfiguration.ApiKey = _congifuration["StripeSetting:SecretKey"];
            shoppingCart.CartTotal = shoppingCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);
            var options = new PaymentIntentCreateOptions
            {
                Amount = (int)(shoppingCart.CartTotal * 100),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var service = new PaymentIntentService();
            PaymentIntent response = service.Create(options);
            shoppingCart.StripePaymentIntentId = response.Id;
            shoppingCart.ClientSecret = response.ClientSecret;
            #endregion

            _response.Result = shoppingCart;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

    }
}
