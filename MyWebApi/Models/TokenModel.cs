namespace MyWebApi.Models
{
    public class TokenModel
    {
        public required string AccessToken { set; get; }
        public required string RefreshToken { set; get; }
    }
}
