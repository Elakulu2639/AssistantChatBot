using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ChatBot.Server.Services;
using ChatBot.Server.Data;
using ChatBot.Server.Models;

namespace ChatBot.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly IChatModelService _chatModelService;

        public ChatController(IChatModelService chatModelService)
        {
            _chatModelService = chatModelService;
            // Initialize with training data
            _chatModelService.TrainModel(TrainingData.GetTrainingData());
        }

        /// <summary>
        /// Sends a message to the chatbot and receives a response
        /// </summary>
        /// <param name="request">The chat message request</param>
        /// <returns>A response from the chatbot</returns>
        /// <response code="200">Returns the chatbot's response</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ChatResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { error = "Message cannot be empty" });
                }

                // Simulate some async processing
                await Task.Delay(100);

                var response = _chatModelService.GetResponse(request.Message);
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }
    }

    /// <summary>
    /// Represents a chat message request
    /// </summary>
    public class ChatRequest
    {
        /// <summary>
        /// The message content to be processed by the chatbot
        /// </summary>
        /// <example>Hello, how can you help me with inventory management?</example>
        public required string Message { get; set; }
    }

    /// <summary>
    /// Represents a chat message response
    /// </summary>
    public class ChatResponse
    {
        /// <summary>
        /// The chatbot's response message
        /// </summary>
        /// <example>I can help you with inventory management. What specific information do you need?</example>
        public required string Response { get; set; }

        /// <summary>
        /// The timestamp when the response was generated
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
} 