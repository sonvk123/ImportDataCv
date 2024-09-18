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
    public class ZaloMessage
    {
        [Key]
        [MaxLength(50)]
        public string ConversationID { get; set; }  // ID của cuộc trò chuyện

        [MaxLength(50)]
        public string MessageID { get; set; }       // ID của tin nhắn

        [MaxLength(50)]
        public string SenderID { get; set; }        // ID của người gửi

        public string MessageContent { get; set; }  // Nội dung tin nhắn

        public DateTime SentTime { get; set; }      // Thời gian gửi tin nhắn

        [MaxLength(20)]
        public string Platform { get; set; }        // Nền tảng (Zalo, Messenger)
    }
}