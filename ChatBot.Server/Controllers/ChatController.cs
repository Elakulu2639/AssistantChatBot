using ChatBot.Server.Models;
using ChatBot.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatModelService _chatModelService;

        public ChatController(IChatModelService chatModelService)
        {
            _chatModelService = chatModelService;
        }

        public class ChatRequest
        {
            public string UserMessage { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
            {
                return BadRequest(new { response = "User message cannot be empty." });
            }

            try
            {
                var result = await _chatModelService.GetChatResponseAsync(request.UserMessage);
                return Ok(new ChatResponse { Response = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ChatResponse { Response = $"Error: {ex.Message}" });
            }
        }
    }
}
