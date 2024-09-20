using ImportDataCv.Server.Models;
using ImportDataCv.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using apiZaloOa.Data;
using apiZaloOa.Services;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ImportDataCv.Server.Models;

namespace ImportDataCv.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class metaController : ControllerBase
    {
        private readonly ApiMetaService _apiService;
        private readonly apiZaloOaContext _metaContext;

        public metaController(apiZaloOaContext metaContext, ApiMetaService apiService)
        {
            _metaContext = metaContext;
            _apiService = apiService;
        }

        [HttpPost("saveTokens")]
        public async Task<IActionResult> SaveTokens([FromBody] MetaToken tokenModel)
        {
            if (tokenModel == null || string.IsNullOrEmpty(tokenModel.UserToken) || string.IsNullOrEmpty(tokenModel.AccessPageToken))
            {
                return BadRequest("Invalid token data.");
            }

            var tokenEntity = new MetaToken
            {
                UserToken = tokenModel.UserToken,
                AccessPageToken = tokenModel.AccessPageToken
            };

            Console.WriteLine("tokenEntity");
            Console.WriteLine(tokenEntity);

            _metaContext.MetaToken.Add(tokenEntity);
            await _metaContext.SaveChangesAsync();

            return Ok(new { message = "Tokens saved successfully." });
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> getConversations()
        {
            string req = await _apiService.conversations();

            // Trả về kết quả dưới dạng JSON
            return Ok(req);
        }

        [HttpGet("conversationId")]
        public async Task<IActionResult> getConversationId(string conversationId)
        {
            object req = await _apiService.conversationId(conversationId);
            Console.WriteLine("req");
            Console.WriteLine(req);

            // Trả về kết quả dưới dạng JSON
            return Ok(req);
        }

        [HttpGet("saveMessages")]
        public async Task<IActionResult> SaveMessages(string conversationId)
        {
            {
                string res = await _apiService.saveMessages(conversationId);
                return Ok(res);
            }
        }

    }

    public class SaveMessagesRequest
    {
        public List<MessengerMessage> Messages { get; set; } = new List<MessengerMessage>();
    }



}
