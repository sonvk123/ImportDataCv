using Azure.Messaging;
using MimeKit;
using System.ComponentModel.DataAnnotations;

namespace apiZaloOa.Models
{
    public class zaloOaTokenModel
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    

    public class ZaloMessages
    {
        public int Id { get; set; }
        public string OaId { get; set; }
        public string uId { get; set; }
        public string uName { get; set; }

    }
    public class ZaloMessagesUserId
    {
        public int Id { get; set; }
        public string OaId { get; set; }
        public string uId { get; set; }
        public string uName { get; set; }
        public string from_avatar { get; set; }
        public string message { get; set; }
        public string message_id { get; set; }



    }
}