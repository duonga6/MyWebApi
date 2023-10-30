using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models
{
    public class CategoryModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
