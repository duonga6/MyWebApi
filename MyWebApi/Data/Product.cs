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
        [MaxLength(50)]
        public required string Name { set; get; }

        public string? Description { set; get; }

        [Range(0, double.MaxValue)]
        public double Price { set; get; }


        public int? CategoryId { set; get; } 
        [ForeignKey("CategoryId")]
        public Category? Category { set; get; }

        public ICollection<OrderDetails> OrderDetails { set; get; } = new List<OrderDetails>();

    }
}
