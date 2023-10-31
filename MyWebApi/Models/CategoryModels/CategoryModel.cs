using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models.CategoryModels
{
    public class CategoryModel
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
