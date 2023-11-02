using MyWebApi.Models.CategoryModels;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models.ProductModels
{
    public class ProductVM
    {
        public int Id { set; get; }
        public required string Name { set; get; }
        public string? Description { set; get; }
        public double Price { set; get; }
        public CategoryVM? Category { get; set; }
    }
}
