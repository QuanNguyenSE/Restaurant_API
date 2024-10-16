﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Repository.IRepository;
using System.Net;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;


        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _response = new APIResponse();
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetCategories()
        {
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAllAsync();
            _response.Result = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCategory(int id)
        {
            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("id not valid");
                return _response;
            }
            Category category = await _unitOfWork.Category.GetAsync(u => u.Id == id);
            _response.Result = _mapper.Map<CategoryDTO>(category);
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
        public async Task<ActionResult<APIResponse>> CreateCategory([FromForm] CategoryCreateDTO categoryDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    Category category = _mapper.Map<Category>(categoryDTO);

                    await _unitOfWork.Category.CreateAsync(category);
                    await _unitOfWork.SaveAsync();

                    _response.IsSuccess = true;
                    _response.Result = _mapper.Map<CategoryDTO>(category);
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetCategory", new { id = category.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Category is not valid");
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateCategory(int id, [FromForm] CategoryUpdateDTO categoryDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (categoryDTO == null || categoryDTO.Id != id)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("Category not found");
                    }

                    Category category = await _unitOfWork.Category.GetAsync(u => u.Id == id, tracking: false);

                    if (category == null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.ErrorMessages.Add("Category not found");
                    }

                    //update
                    category = _mapper.Map<Category>(categoryDTO);


                    await _unitOfWork.Category.UpdateAsync(category);
                    await _unitOfWork.SaveAsync();

                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Category is not valid");
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteCategory(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Id not valid");
                }

                Category category = await _unitOfWork.Category.GetAsync(u => u.Id == id);

                if (category == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Category not found");
                    return NotFound(_response);
                }

                await _unitOfWork.Category.RemoveAsync(category);
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
