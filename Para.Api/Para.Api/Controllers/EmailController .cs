using Microsoft.AspNetCore.Mvc;
using Para.Bussiness.RabbitMQ;
using System.Text.Json;

namespace Para.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;

        public EmailController(RabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        [Route("send-email")]
        public IActionResult SendEmail([FromBody] EmailDetails emailDetails)
        {
            var message = JsonSerializer.Serialize(emailDetails);
            _rabbitMQService.Publish(message);
            return Ok("Email queued for sending.");
        }
    }

    public class EmailDetails
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
