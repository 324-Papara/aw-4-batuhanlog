using Para.Bussiness.Email;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Para.Bussiness.RabbitMQ
{
    public class EmailJob
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly EmailService _emailService;

        public EmailJob(RabbitMQService rabbitMQService, EmailService emailService)
        {
            _rabbitMQService = rabbitMQService;
            _emailService = emailService;
        }

        public void Execute()
        {
            var consumer = new EventingBasicConsumer(_rabbitMQService.Channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var emailDetails = JsonSerializer.Deserialize<EmailDetails>(message);
                await _emailService.SendEmailAsync(emailDetails.To, emailDetails.Subject, emailDetails.Body);
            };
            _rabbitMQService.Channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);
        }
    }

    public class EmailDetails
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
