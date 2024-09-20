using Microsoft.AspNetCore.Mvc;
using apiZaloOa.Services;
using apiZaloOa.Models;
using Azure.Core;
using System.Net.Http.Headers;
using apiZaloOa.Data;
using Microsoft.EntityFrameworkCore;
using Azure;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace apiZaloOa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class zaloController : ControllerBase
    {
        private readonly ApiZaloService _apiService;
        private readonly apiZaloOaContext _context;
        public zaloController(ApiZaloService apiService, apiZaloOaContext apiZaloOa_Context)
        {
            _apiService = apiService;
            _context = apiZaloOa_Context;
        }

        // GET: api/sendmessage/AccessToken
        [HttpGet("AccessToken")]
        public async Task<IActionResult> AccessToken(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                // Nếu tham số code bị thiếu hoặc null, trả về lỗi BadRequest
                return BadRequest("Code is required.");
            }

            try
            {
                // Gọi ApiService để lấy access token
                var accessToken = await _apiService.AccessToken(code);

                if (accessToken.StartsWith("Error:"))
                {
                    // Nếu accessToken trả về là lỗi, trả về thông tin lỗi từ ApiService
                    return StatusCode(500, accessToken); // Lỗi 500 với thông báo lỗi
                }

                // Nếu lấy được access token thành công, trả về kết quả
                return Ok(new { AccessToken = accessToken });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi bất ngờ xảy ra, trả về lỗi 500 và thông tin chi tiết
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> getcConversations()
        {
            var responseBody = await _apiService.getMessages();
            var response = responseBody;
            return Ok(response);
        }
        [HttpGet("getMessages")]
        public async Task<IActionResult> getMessages()
        {
            var responseBody = await _apiService.getMessages();
            var response = responseBody;
            return Ok(response);

        }
    
        // GET: api/sendmessage/messageUserId
        [HttpGet("messageUserId")]
        public async Task<IActionResult> messageUserId(string userId)
        {

            var responseBody = await _apiService.messageUserId(userId);
            var response = responseBody;
            return Ok(response);

        }
    }
}