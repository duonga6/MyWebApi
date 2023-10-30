using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Data
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int Id { set; get; }

        [Required]
        public string Name { set; get; }

        public string Description { set; get; }

        [Range(0, double.MaxValue)]
        public double Price { set; get; }



        public int? CategoryId { set; get; } 

        [ForeignKey("CategoryId")]
        public Category? Category { set; get; }
    }
}
