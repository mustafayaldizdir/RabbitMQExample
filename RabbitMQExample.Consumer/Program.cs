using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace RabbitMQExample.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://whfutxom:DnyMfdULBaRGhQvC6mQcmefv2w49ezdD@moose.rmq.cloudamqp.com/whfutxom");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false);
            var queueName = "direct-queue-Critical";

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine("Loglar dinleniyor...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Gelen Mesaj: " + message);
                //File.AppendAllText("log-ciritcal.txt", message + "\n");
                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
