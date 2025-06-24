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
        public async Task<IActionResult> SendMessage(MessageSenderDTO messageDTO)
        {
            if (messageDTO == null)
            {
                return BadRequest("Message data is null.");
            }

            if (string.IsNullOrWhiteSpace(messageDTO.Text))
            {
                return BadRequest("Message text is empty.");
            }

            if (messageDTO.SenderId == Guid.Empty || messageDTO.RecipientId == Guid.Empty)
            {
                return BadRequest("Invalid sender or recipient ID.");
            }

            await _messageService.MessageSender(messageDTO.SenderId, messageDTO.RecipientId, messageDTO.Text );

            return   RedirectToAction("GetMessage", "Message", 
                new {
                    senderId = messageDTO.SenderId,
                    recipientId = messageDTO.RecipientId
                });
        }

       
        [HttpGet]
        public async Task<IActionResult> GetMessage(Guid senderId, Guid recipientId)
        {

            var messages = await _messageService.GetMessage(senderId, recipientId);
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
