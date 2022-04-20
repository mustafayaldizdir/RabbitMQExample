using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQExample.Producer
{
    public enum LogNames
    {
        Critical = 1,
        Error=2,
        Warning=3,
        Info=4
    }
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://whfutxom:DnyMfdULBaRGhQvC6mQcmefv2w49ezdD@moose.rmq.cloudamqp.com/whfutxom");


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare("hello-queue", true, false, false);
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route-{x}";
                var quueName = $"direct-queue-{x}";
                channel.QueueDeclare(quueName, true, false, false);
                channel.QueueBind(quueName, "logs-direct", routeKey,null);
            });


            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log = (LogNames)new Random().Next(1, 5);
                string message = $"log-type: {log}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                var routeKey = $"route-{log}";
                channel.BasicPublish("logs-direct",routeKey,null,messageBody);
                Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
            });

           

            Console.ReadLine();


        }
    }
}
