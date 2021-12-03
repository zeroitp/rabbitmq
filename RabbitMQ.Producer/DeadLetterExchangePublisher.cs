using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Producer
{
    public static class DeadLetterExchangePublisher
    {
        public static void Publish(IModel channel)
        {
            //declare deadletter exchange and queue
            const string deadLetterExchangeName = "demo-deadletter-exchange";
            const string deadLetterQueueName = "demo-deadletter-queue";
            const string deadLetterRoutingKey = "demo-deadletter.#";
            DeadLetterExchange.DeclareDeadLetterExchange(channel, deadLetterExchangeName, deadLetterQueueName, deadLetterRoutingKey);

            //decalre demo exchange and queue
            const string demoExchangeName = "demo-fanout-to-dlx-exchange";
            const string demoQueueName = "demo-fanout-to-dlx-queue";
            const string demoRoutingKey = "demo-direct-dlx.#";
            var exchangeOpts = new Dictionary<string, object>
            {
                {"x-message-ttl", 15000 }
            };
            channel.ExchangeDeclare(demoExchangeName, ExchangeType.Fanout, arguments: exchangeOpts);
            var count = 0;

            var queueOpts = new Dictionary<string, object>
            {
                {"x-message-ttl", 15000 },
                {"x-dead-letter-exchange", deadLetterExchangeName }
            };            
            channel.QueueDeclare(demoQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueOpts);
            channel.QueueBind(demoQueueName, demoExchangeName, demoRoutingKey);

            while (true)
            {
                var message = new { Name = $"Demo Fanout to DLX Producer {count}", Message = $"Id: { Guid.NewGuid() } request time: { DateTime.Now.ToString("HH:mm:ss:ffff") }" };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(demoExchangeName, "demo-direct-dlx.test", null, body);

                Console.WriteLine($"Dead letter exchange producer current index {count} \n");

                count++;
                Thread.Sleep(1000);
            }
        }
    }
}
