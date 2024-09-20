using Microsoft.AspNetCore.Mvc;
using apiZaloOa.Services;
using apiZaloOa.Models;
using Azure.Core;
using System.Net.Http.Headers;
using apiZaloOa.Data;
using Microsoft.EntityFrameworkCore;
using Azure;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using TL;

namespace ImportDataCv.Server.Controllers
{
    public class teleController : Controller
    {
        private readonly ApiTeleService _apiTeleService;
        private readonly apiZaloOaContext _context;
        public teleController(ApiTeleService apiTeleService, apiZaloOaContext apiZaloOa_Context)
        {
            _apiTeleService = apiTeleService;
            _context = apiZaloOa_Context;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> DoLogin(string loginInfo)
        {
       
                string isSuccess = await _apiTeleService.DoLogin(loginInfo);
             
           
                    return Ok(isSuccess);
                

        }

        [HttpGet("Messages_GetAllDialogs")]
        public async Task<IActionResult> Messages_GetAllDialogs()
        {
            Object chatHistory = await _apiTeleService.MessagesGetAllDialogs();
            return Ok(chatHistory);
        }

        //[HttpGet("GetChatHistory")]
        //public async Task<IActionResult> GetChatHistory(long user_ID)
        //{
        //    List<string> chatHistory = await _apiTeleService.GetChatHistory(user_ID);
        //    return Ok(chatHistory);
        //}
        //[HttpGet("GetChatHistoryAsync")]
        //public async Task<IActionResult> GetChatHistoryAsync(InputPeerUser peerInput)
        //{
        //    List<string> chatHistory = await _apiTeleService.GetChatHistoryAsync(peerInput);
        //    return Ok(chatHistory);
        //}
    }
}