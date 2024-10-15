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
        [HttpGet("{id:int}", Name = "GetMenuItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetMenuItem(int id)
        {
            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("id not valid");
                return _response;
            }
            var menuItem = await _unitOfWork.MenuItem.GetAsync(u => u.Id == id);
            _response.Result = _mapper.Map<MenuItemDTO>(menuItem);
            if (_response.Result == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Not found");
                return _response;
            }
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemDTO.Image == null || menuItemDTO.Image.Length == 0)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("Image is require");
                    }

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    //set filename
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(menuItemDTO.Image.FileName);
                    //path
                    string menuItemPath = Path.Combine(wwwRootPath, @"images\menuitem\");

                    using (var fileStream = new FileStream(Path.Combine(menuItemPath, fileName), FileMode.Create))
                    {
                        menuItemDTO.Image.CopyTo(fileStream);
                    }

                    MenuItem menuItem = _mapper.Map<MenuItem>(menuItemDTO);
                    menuItem.ImageUrl = @"\images\menuitem\" + fileName;

                    await _unitOfWork.MenuItem.CreateAsync(menuItem);
                    await _unitOfWork.SaveAsync();

                    _response.IsSuccess = true;
                    _response.Result = menuItem;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Menu Item is not valid");
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemDTO == null || menuItemDTO.Id != id)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("MenuItem not found");
                    }

                    MenuItem menuItem = await _unitOfWork.MenuItem.GetAsync(u => u.Id == id);

                    if (menuItem == null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.ErrorMessages.Add("MenuItem not found");
                    }

                    //update
                    menuItem.Name = menuItemDTO.Name;
                    menuItem.Price = menuItemDTO.Price;
                    menuItem.Category = menuItemDTO.Category;
                    menuItem.SpecialTag = menuItemDTO.SpecialTag;
                    menuItem.Description = menuItemDTO.Description;

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (menuItemDTO.Image != null && menuItemDTO.Image.Length > 0)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, menuItem.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        //set filename
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(menuItemDTO.Image.FileName);
                        //path
                        string menuItemPath = Path.Combine(wwwRootPath, @"images\menuitem\");
                        using (var fileStream = new FileStream(Path.Combine(menuItemPath, fileName), FileMode.Create))
                        {
                            menuItemDTO.Image.CopyTo(fileStream);
                        }
                        menuItem.ImageUrl = @"\images\menuitem\" + fileName;
                    }
                    await _unitOfWork.MenuItem.UpdateAsync(menuItem);
                    await _unitOfWork.SaveAsync();

                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Menu Item is not valid");
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteMenuItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Id not valid");
                }

                MenuItem menuItem = await _unitOfWork.MenuItem.GetAsync(u => u.Id == id);

                if (menuItem == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("MenuItem not found");
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                var oldImagePath = Path.Combine(wwwRootPath, menuItem.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                await _unitOfWork.MenuItem.RemoveAsync(menuItem);
                await _unitOfWork.SaveAsync();

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


    }
}
