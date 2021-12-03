using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Producer
{
    public static class DeadLetterExchange
    {
        public static void DeclareDeadLetterExchange(IModel channel, string exchangeName, string queueName, string routingKey)
        {
            //declare exchange
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", 30000 }
            };

            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, arguments: ttl);

            //declare queue
            channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueBind(queueName, exchangeName, routingKey);
            channel.BasicQos(0, 10, false);
        }
    }
}
