using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Producer
{
    static class TopicExchangePublisher
    {
        public static void Publish(IModel channel)
        {
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", 30000 }
            };
            channel.ExchangeDeclare("demo-topic-exchange", ExchangeType.Topic, arguments: ttl);

            while (true)
            {
                var message = new { Name = "Topic Producer", Message = $"Id: { Guid.NewGuid() } request time: { DateTime.Now.ToString("HH:mm:ss:ffff") }" };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish("demo-topic-exchange", "user.update", null, body);
                Thread.Sleep(1000);
            }
        }
    }
}
