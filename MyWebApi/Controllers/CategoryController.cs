using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Data;
using MyWebApi.Models;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CategoryController(MyDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _context.Categories;
            return Ok(categories.ToList());
        }

        [HttpGet("id")]
        public IActionResult Get(int id)
        {
            try
            {
                var category = _context.Categories.SingleOrDefault(c => c.Id == id);
                if (category == null)
                {
                    return NotFound();
                }

                return Ok(category);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create(CategoryModel model)
        {
            try
            {
                var category = new Category()
                {
                    Name = model.Name,
                    Description = model.Description
                };

                _context.Categories.Add(category);
                _context.SaveChanges();

                return Ok(category);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CategoryModel model)
        {
            try
            {
                var category = _context.Categories.SingleOrDefault(c => c.Id == id);
                if (category == null)
                    return NotFound();

                category.Name = model.Name;
                category.Description = model.Description;

                _context.SaveChanges();

                return Ok(category);

            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var category = _context.Categories.SingleOrDefault(c => c.Id == id);
                if (category == null)
                    return NotFound();

                _context.Categories.Remove(category);
                _context.SaveChanges();

                return Ok();

            }
            catch
            {
                return BadRequest();

            }
        }
    }
}
