using ChatbotAI_BE.Models;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chats;
        public ChatController(IChatService chats) => _chats = chats;

        [HttpGet("world")]
        public async Task<ActionResult<IReadOnlyList<Message>>> GetWorld([FromQuery] int limit = 200)
            => Ok(await _chats.GetWorldAsync(limit));

        [HttpGet("direct")]
        public async Task<ActionResult<IReadOnlyList<Message>>> GetDirect([FromQuery] string user1, [FromQuery] string user2, [FromQuery] int limit = 200)
            => Ok(await _chats.GetDirectAsync(user1, user2, limit));

        [HttpGet("group/{groupName}")]
        public async Task<ActionResult<IReadOnlyList<Message>>> GetGroup(string groupName, [FromQuery] int limit = 200)
            => Ok(await _chats.GetGroupAsync(groupName, limit));
    }
}
