using MyWebApi.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyWebApi.Models.ProductModels
{
    public class ProductModel
    {
        [Required]
        [MaxLength(50)]
        public required string Name { set; get; }

        public string? Description { set; get; }

        [Range(0, double.MaxValue)]
        public double Price { set; get; }

        public int? CategoryId { set; get; }
    }
}
