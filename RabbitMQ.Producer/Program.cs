using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Producer
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
        //    FanoutExchangePublisher.Publish(channel);
        //    //DirectExchangePublisher.Publish(channel);
        //    //TopicExchangePublisher.Publish(channel);
        //    //HeaderExchangePublisher.Publish(channel);
        //    //QueueProducer.Publish(channel);
        //}

        public static void Main(string[] args)
        {
            Console.WriteLine("RPC Client");
            for (int i = 0; i < 10; i++)
            {
                string message = $"Test RPC {i}";
                Task t = InvokeAsync(message);
                t.Wait();
            }
            Console.ReadLine();
        }

        private static async Task InvokeAsync(string message)
        {
            var rpcClient = new RpcClient();

            Console.WriteLine($"Requesting message: '{message}' ");
            var response = await rpcClient.CallAsync(message);
            Console.WriteLine($"--> Got '{response}'");
        }
    }
}
