using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQExample.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
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
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            
            var queueName = channel.QueueDeclare().QueueName; 

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "any");
            
            channel.QueueBind(queueName, "header-exchange",String.Empty,headers);

            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine("Headerlar dinleniyor...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Product product = JsonSerializer.Deserialize<Product>(message);

                Thread.Sleep(1500);

                Console.WriteLine($"Gelen Mesaj:  {product.Id}-{product.Name}-{product.Price}-{product.Stock}");

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
