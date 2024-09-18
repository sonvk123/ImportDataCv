using System.ComponentModel.DataAnnotations;

namespace apiZaloOa.Models
{
    public class zaloOaTokenModel
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
