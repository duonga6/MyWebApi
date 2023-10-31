using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models.CategoryModels;
using MyWebApi.Services.CategoryServices;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService) 
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult GetAll ()
        {
            try
            {
                return Ok(_categoryService.GetAll());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById (int id)
        {
            try
            {
                var category = _categoryService.GetById(id);
                if (category == null)
                {
                    return NotFound();
                }

                return Ok(category);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public IActionResult Create (CategoryModel model)
        {
            try
            {
                return Ok(_categoryService.Create(model));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update (int id, CategoryVM model)
        {
            if (id != model.Id)
                return NotFound();
            try
            {
                return Ok(_categoryService.Update(model));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete (int id)
        {
            try
            {
                _categoryService.Delete(id);
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
