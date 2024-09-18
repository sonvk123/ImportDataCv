using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // Cần thêm thư viện này để xử lý JSON
using apiZaloOa.Models;
using Microsoft.EntityFrameworkCore;
using apiZaloOa.Data;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace apiZaloOa.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly apiZaloOaContext _context;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, apiZaloOaContext context, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _context = context;
            _logger = logger;
        }
        public async Task<string> AccessToken(string code)
        {
            // URL và thông tin cần thiết
            string url = "https://oauth.zaloapp.com/v4/oa/access_token";
            string secretKey = "MO44iLB5s8n2k8DKSDCY";
            long appId = 3149800129752642817;

            // Dictionary chứa các tham số sẽ gửi
            var parameters = new Dictionary<string, string>
            {
                { "app_id", appId.ToString() },  // Chuyển long thành string
                { "code", code },
                { "grant_type", "authorization_code" }
            };

            // Tạo nội dung form-urlencoded từ dictionary
            var content = new FormUrlEncodedContent(parameters);

            content.Headers.Add("secret_key", secretKey); // Thêm secret_key vào header

            try
            {
                // Gửi yêu cầu POST
                var response = await _httpClient.PostAsync(url, content);

                // Đọc kết quả trả về dưới dạng chuỗi
                string responseString = await response.Content.ReadAsStringAsync();

                // Parse JSON để kiểm tra kết quả hoặc trả về response trực tiếp
                var jsonResponse = JObject.Parse(responseString);

                _logger.LogInformation("jsonResponse: {JsonResponse}", jsonResponse);

                string accessToken = jsonResponse["access_token"]?.ToString();
                string refreshToken = jsonResponse["refresh_token"]?.ToString();


                // Kiểm tra nếu không có accessToken hoặc refreshToken
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogError("Failed to retrieve accessToken or refreshToken.");
                    return "Error: Missing accessToken or refreshToken.";
                }

                // Cập nhật token trong cơ sở dữ liệu
                // Kiểm tra xem cơ sở dữ liệu có token nào không
                var existingTokens = await _context.zaloOaTokenModel.ToListAsync();

                if (existingTokens.Any()) // Nếu có token cũ
                {
                    // Xóa tất cả các token cũ
                    _context.zaloOaTokenModel.RemoveRange(existingTokens);

                    // Lưu thay đổi để xóa token cũ
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Old tokens removed from the database.");
                }

                // Tạo mô hình token mới
                var newToken = new zaloOaTokenModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                // Thêm token mới vào cơ sở dữ liệu
                _context.zaloOaTokenModel.Add(newToken);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                _logger.LogInformation("accessToken: {AccessToken}", accessToken);
                _logger.LogInformation("refreshToken: {RefreshToken}", refreshToken);

                // Trả về accessToken
                return accessToken;

            }
            catch (HttpRequestException ex)
            {
                return $"Request error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
        public async Task<string> getMessages()
        {
            var token = await _context.zaloOaTokenModel.FirstOrDefaultAsync();
            string accessToken = token.AccessToken;

            // URL của API
            string url = "https://openapi.zalo.me/v2.0/oa/listrecentchat?data={\"offset\":0,\"count\":5}";

            // Tạo đối tượng HttpRequestMessage
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Thêm header 'access_token'
            request.Headers.Add("access_token", accessToken);

            // Thêm query string
            var queryParams = "?data=" + Uri.EscapeDataString("{\"offset\":0,\"count\":5}");
            request.RequestUri = new Uri(url + queryParams);

            // Gửi yêu cầu và nhận phản hồi
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung phản hồi
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
            {
                // Xử lý lỗi nếu có
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }
        public async Task<string> messageUserId(string userId)
        {
            var token = await _context.zaloOaTokenModel.FirstOrDefaultAsync();
            string accessToken = token.AccessToken;

            // URL của API
           
            string url = $"https://openapi.zalo.me/v2.0/oa/conversation?data={{\"user_id\":\"{userId}\",\"offset\":0,\"count\":5}}";

            // Tạo đối tượng HttpRequestMessage
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Thêm header 'access_token'
            request.Headers.Add("access_token", accessToken);

            // Thêm query string
            var queryParams = "?data=" + Uri.EscapeDataString("{\"offset\":0,\"count\":5}");
            request.RequestUri = new Uri(url + queryParams);

            // Gửi yêu cầu và nhận phản hồi
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung phản hồi
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
            {
                // Xử lý lỗi nếu có
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }
    }

}