using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Repository.IRepository;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;


        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _response = new APIResponse();
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetMenuItems()
        {
            IEnumerable<MenuItem> menuItems = await _unitOfWork.MenuItem.GetAllAsync();
            _response.Result = _mapper.Map<IEnumerable<MenuItemDTO>>(menuItems);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }
        //[HttpGet("{id:int}", Name = "GetMenuItem")]
        //public async Task<ActionResult<APIResponse>> GetMenuItem(int id)
        //{
        //    if (id == 0)
        //    {
        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.ErrorMessages.Add("id not valid");
        //        return _response;
        //    }
        //    _response.Result = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);
        //    if (_response.Result == null)
        //    {
        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.NotFound;
        //        _response.ErrorMessages.Add("Not found");
        //        return _response;
        //    }
        //    _response.StatusCode = HttpStatusCode.OK;
        //    return _response;
        //}

        //[HttpPost]
        //public async Task<ActionResult<APIResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemDTO)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            string wwwRootPath = _webHostEnvironment.WebRootPath;
        //            if (menuItemDTO.Image == null || menuItemDTO.Image.Length == 0)
        //            {
        //                _response.IsSuccess = false;
        //                _response.StatusCode = HttpStatusCode.BadRequest;
        //                _response.ErrorMessages.Add("Image is require");
        //            }

        //            //set filename
        //            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(menuItemDTO.Image.FileName);
        //            //path
        //            string menuItemPath = Path.Combine(wwwRootPath, @"images\menuitem\");

        //            using (var fileStream = new FileStream(Path.Combine(menuItemPath, fileName), FileMode.Create))
        //            {
        //                menuItemDTO.Image.CopyTo(fileStream);
        //            }

        //            MenuItem menuItem = _mapper.Map<MenuItem>(menuItemDTO);
        //            menuItem.ImageUrl = @"\images\menuitem\" + fileName;

        //            _db.MenuItems.Add(menuItem);
        //            _db.SaveChanges();

        //            _response.IsSuccess = true;
        //            _response.Result = menuItem;
        //            _response.StatusCode = HttpStatusCode.Created;
        //            return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, _response);
        //        }
        //        else
        //        {
        //            _response.IsSuccess = false;
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.ErrorMessages.Add("Menu Item is not valid");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.ErrorMessages = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult<APIResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemDTO)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (menuItemDTO == null || menuItemDTO.Id != id)
        //            {
        //                _response.IsSuccess = false;
        //                _response.StatusCode = HttpStatusCode.BadRequest;
        //                //_response.ErrorMessages.Add("Image is require");
        //            }

        //            MenuItem menuItem = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);

        //            if (menuItem == null)
        //            {
        //                _response.IsSuccess = false;
        //                _response.StatusCode = HttpStatusCode.BadRequest;
        //                //_response.ErrorMessages.Add("Image is require");
        //            }

        //            //update
        //            menuItem = _mapper.Map<MenuItem>(menuItemDTO);

        //            string wwwRootPath = _webHostEnvironment.WebRootPath;
        //            if (menuItemDTO.Image != null || menuItemDTO.Image.Length > 0)
        //            {
        //                var oldImagePath = Path.Combine(wwwRootPath, menuItem.ImageUrl.TrimStart('\\'));
        //                if (System.IO.File.Exists(oldImagePath))
        //                {
        //                    System.IO.File.Delete(oldImagePath);
        //                }
        //                //set filename
        //                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(menuItemDTO.Image.FileName);
        //                //path
        //                string menuItemPath = Path.Combine(wwwRootPath, @"images\menuitem\");
        //                using (var fileStream = new FileStream(Path.Combine(menuItemPath, fileName), FileMode.Create))
        //                {
        //                    menuItemDTO.Image.CopyTo(fileStream);
        //                }
        //                menuItem.ImageUrl = @"\images\menuitem\" + fileName;
        //            }
        //            _db.MenuItems.Update(menuItem);
        //            _db.SaveChanges();

        //            _response.IsSuccess = true;
        //            _response.Result = menuItem;
        //            _response.StatusCode = HttpStatusCode.NoContent;
        //        }
        //        else
        //        {
        //            _response.IsSuccess = false;
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.ErrorMessages.Add("Menu Item is not valid");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.ErrorMessages = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}
    }
}
