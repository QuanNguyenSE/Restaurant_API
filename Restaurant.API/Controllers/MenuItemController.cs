using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected APIResponse _response;

        public MenuItemController(ApplicationDbContext db)
        {
            _db = db;
            _response = new APIResponse();
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllAsync()
        {
            _response.Result = await _db.MenuItems.ToListAsync();
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

    }
}
