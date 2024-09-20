using TL;
using WTelegram;
using System.Collections.Generic;
using apiZaloOa.Data;
using apiZaloOa.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using MimeKit;
using Org.BouncyCastle.Crypto;
using ImportDataCv.Server.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace apiZaloOa.Services
{

    public class ApiMetaService
    {

        private readonly HttpClient _httpClient;
        private readonly apiZaloOaContext _context;

        // Biến toàn cục để lưu trữ conversationResponse
        private ConversationResponse _conversationResponse;


        public ApiMetaService(HttpClient httpClient, apiZaloOaContext context, ILogger<ApiZaloService> logger)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<string> conversations()
        {
            string clientIdPage = "130700533182452";
            var token = await _context.MetaToken.FirstOrDefaultAsync();
            string sccessPageToken = token.AccessPageToken;

            var url = $"https://graph.facebook.com/v20.0/{clientIdPage}/conversations?platform=MESSENGER&access_token={sccessPageToken}";
            // Tạo đối tượng HttpRequestMessage
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Gửi yêu cầu và nhận phản hồi
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung phản hồi
                string responseBodyString = await response.Content.ReadAsStringAsync();

                return responseBodyString;
            }
            else
            {
                // Xử lý lỗi nếu có
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }

        // lấy thông tin chi tiết 1 cuộc trò chuyện 
        public async Task<ConversationResponse> conversationId(string conversationId)
        {
            var token = await _context.MetaToken.FirstOrDefaultAsync();
            string sccessPageToken = token.AccessPageToken;
            var url = $"https://graph.facebook.com/v20.0/{conversationId}?fields=messages&access_token={sccessPageToken}";

            // Tạo đối tượng HttpRequestMessage
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Gửi yêu cầu và nhận phản hồi
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung phản hồi
                string responseBodyString = await response.Content.ReadAsStringAsync();
                Type type = responseBodyString.GetType();

                // Phân tích JSON để lấy danh sách các ID tin nhắn
                var jsonData = JObject.Parse(responseBodyString);
                var messages = jsonData["messages"]["data"];

                List<NewMessageObject> newMessages = new List<NewMessageObject>();

                foreach (var message in messages)
                {
                    string messageId = message["id"].ToString();

                    // Gọi hàm lấy thông tin chi tiết về tin nhắn
                    var messageDetailJson = await getMessageId(messageId);
                    var messageDetail = JObject.Parse(messageDetailJson);
                    // Tạo một đối tượng mới từ thông tin chi tiết
                    var newMessage = new NewMessageObject
                    {
                        Id = messageDetail["id"].ToString(),
                        CreatedTime = messageDetail["created_time"].ToString(),
                        FromUserName = messageDetail["from"]["name"].ToString(),
                        FromEmail = messageDetail["from"]["email"]?.ToString(),
                        FromId = messageDetail["from"]["id"].ToString(),
                        Message = messageDetail["message"].ToString()
                    };
                    // Lấy thông tin của người nhận
                    var toData = messageDetail["to"]["data"]?.First;
                    if (toData != null)
                    {
                        newMessage.ToUserName = toData["name"]?.ToString();
                        newMessage.ToEmail = toData["email"]?.ToString();
                        newMessage.ToId = toData["id"]?.ToString();
                    }
                    newMessages.Add(newMessage);
                }
                // Tạo đối tượng trả về
                _conversationResponse = new ConversationResponse
                {
                    Messages = new MessagesResponse { Data = newMessages },
                    Id = conversationId // Hoặc bạn có thể lấy ID từ jsonData nếu có
                };
                return _conversationResponse;
            }

            // Trường hợp không thành công
            return _conversationResponse;
        }

        // Lấy thông tin chi tiết tin nhắn theo ID
        public async Task<string> getMessageId(string messageId)
        {
            var token = await _context.MetaToken.FirstOrDefaultAsync();
            if (token == null)
            {
                throw new Exception("Token not found");
            }

            string successPageToken = token.AccessPageToken;
            var url = $"https://graph.facebook.com/v20.0/{messageId}?fields=id,created_time,from,to,message&access_token={successPageToken}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBodyString = await response.Content.ReadAsStringAsync();


                return responseBodyString;
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }
        public async Task<string> saveMessages(string converId)
        {


            var messagesResponse = await conversationId(converId);
            var messages = messagesResponse.Messages.Data;



            foreach (var message in messages)
            {
            Console.WriteLine("message.CreatedTime.ToString();");
            Console.WriteLine(message.CreatedTime);
                string _ConversationId = converId;
                string _MessageId = message.Id;  // Giả sử ID là chuỗi
                DateTime _MessageCreatedTime = DateTime.Parse(message.CreatedTime);
                string _FromId = message.FromId;
                string _FromName = message.FromUserName;
                string _FromEmail = message.FromEmail;
                string _ToId = message.ToId;
                string _ToName = message.ToUserName;
                string _ToEmail = message.ToEmail;
                string _MessageText = message.Message;


                //Tạo mô hình ZaloMessage mới
                var newMetaMessageUserId = new MessengerMessage
                {
                    ConversationId = _ConversationId,
                    MessageCreatedTime = _MessageCreatedTime,
                    MessageId = _MessageId,
                    FromId = _FromId,
                    FromName = _FromName,
                    FromEmail = _FromEmail,
                    ToId = _ToId,
                    ToName = _ToName,
                    ToEmail = _ToEmail,
                    MessageText = _MessageText
                };

                // Thêm ZaloMessage mới vào cơ sở dữ liệu
                _context.MessengerMessage.Add(newMetaMessageUserId);

                try
                {
                    // Code cập nhật cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.InnerException?.Message);  // Xem thông tin chi tiết
                }

            }
            var res = "oke";
            return res;
        }

    }
    // Đối tượng mới để lưu thông tin
}
