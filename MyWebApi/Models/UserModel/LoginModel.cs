using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models.UserModel
{
    public class LoginModel
    {
        [Required]
        [StringLength(16)]
        public required string UserName { set; get; }
        [Required]
        [StringLength(16)]
        public required string Password { set; get; }
    }
}
