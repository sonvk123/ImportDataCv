using Microsoft.AspNetCore.Mvc;
using apiZaloOa.Services;
using apiZaloOa.Models;

namespace apiZaloOa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendMessageController : ControllerBase
    {
        private readonly ApiService _apiService;

        public SendMessageController(ApiService apiService)
        {
            _apiService = apiService;
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
    }
}