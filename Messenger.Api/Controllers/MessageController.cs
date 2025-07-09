using Messenger.App.DTOs;
using Messenger.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers
{

    public class MessageController : Controller
    {
        private readonly MessageService _messageService;
        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageSenderDTO enderDto)
        {
            if (enderDto == null || string.IsNullOrWhiteSpace(enderDto.Text))
                return BadRequest("Message is empty");
            string chatRoom = _messageService.GenerateChatRoomName(enderDto.SenderId, enderDto.RecipientId);
            await _messageService.SendMessageAsync(chatRoom ,enderDto.SenderId, enderDto.RecipientId, enderDto.Text);

            return Ok();
        }



        [HttpGet]
        public async Task<IActionResult> GetMessage(Guid senderId, Guid recipientId)
        {

            var messages = await _messageService.GetMessagesAsync(senderId, recipientId);

            if (messages == null || messages.Count == 0)
            {
                ViewBag.NoMessages = true;
            }

            ViewBag.SenderId = senderId;
            ViewBag.RecipientId = recipientId;

            return View(messages);
        }
    }
}
