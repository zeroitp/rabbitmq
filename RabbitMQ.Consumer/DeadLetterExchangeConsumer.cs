using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Consumer
{
    public static class DeadLetterExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) => {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"{message} recieved time: {DateTime.Now.ToString("HH:mm:ss:ffff")}");
            };

            channel.BasicConsume("demo-deadletter-queue", true, consumer);
            Console.WriteLine("Dead letter exchange consumer started");
            Console.ReadLine();
        }
    }
}
