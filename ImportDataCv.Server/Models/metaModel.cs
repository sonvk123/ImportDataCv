using System;

namespace ImportDataCv.Server.Models
{
    public partial class MessengerMessage
    {
        public int Id { get; set; }
        public string ConversationId { get; set; } = null!;

        //public DateTime? UpdatedTime { get; set; }

        public string? MessageId { get; set; }

        public DateTime? MessageCreatedTime { get; set; }

        public string? FromId { get; set; }

        public string? FromName { get; set; }

        public string? FromEmail { get; set; }

        public string? ToId { get; set; }

        public string? ToName { get; set; }

        public string? ToEmail { get; set; }

        public string? MessageText { get; set; }
    }
    public partial class MetaToken
    {
        public int Id { get; set; }

        public string AccessPageToken { get; set; } = null!;

        public string UserToken { get; set; } = null!;
    }
    public class NewMessageObject
    {
        public string Id { get; set; }
        public string CreatedTime { get; set; }
        public string FromUserName { get; set; }
        public string FromEmail { get; set; }
        public string FromId { get; set; }
        public string ToUserName { get; set; }
        public string ToId { get; set; }
        public string ToEmail { get; set; }
        public string Message { get; set; }
    }
    public class ConversationResponse
    {
        public MessagesResponse Messages { get; set; }
        public string Id { get; set; }
    }
    public class MessagesResponse
    {
        public List<NewMessageObject> Data { get; set; }
    }
}