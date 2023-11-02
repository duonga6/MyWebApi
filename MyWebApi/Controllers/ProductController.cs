using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models.ProductModels;
using MyWebApi.Services.ProductServices;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAll(string? search, double? from, double? to, string? sortBy, int page = 1)
        {
            try
            {
                var products = _productService.Get(search, from, to, sortBy, page);
                return Ok(new
                {
                    pageIndex = products.PageIndex,
                    totalPage = products.TotalPage,
                    products
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("Id")]
        public IActionResult GetById(int Id)
        {
            try
            {
                var product = _productService.GetById(Id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public IActionResult Create(ProductModel model)
        {
            try
            {
                return Ok(_productService.Create(model));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("Id")]
        public IActionResult Update(int Id, ProductVM model)
        {
            if (Id != model.Id) { return NotFound(); }

            try
            {
                return Ok(_productService.Update(model));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("Id")]
        public IActionResult Delete(int Id)
        {
            try
            {
                _productService.Delete(Id);
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
