using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Data
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(16)]
        public required string UserName { set; get; }
        [Required]
        [StringLength(16)]
        public required string Password { set; get; }
        public string? Email { set; get; }
        public string? FullName { set; get; }
    }
}
