using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Consumer
{
    static class Program
    {
        //static void Main(string[] args)
        //{
        //    var factory = new ConnectionFactory
        //    {
        //        Uri = new Uri("amqp://guest:guest@localhost:5672")
        //    };
        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();
        //    FanoutExchangeConsumer.Consume(channel);
        //    //TopicExchangeConsumer.Consume(channel);
        //    //HeaderExchangeConsumer.Consume(channel);
        //    //DirectExchangeConsumer.Consume(channel);
        //    //QueueConsumer.Consume(channel);
        //}

        public static void Main()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body.ToArray();
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);                        
                        Console.WriteLine($"Got '{message}'");
                        response = $"{message} + CorrelationId: {props.CorrelationId}";
                        Console.WriteLine($"--> Response: '{response}'");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                Console.ReadLine();
            }
        }
    }
}