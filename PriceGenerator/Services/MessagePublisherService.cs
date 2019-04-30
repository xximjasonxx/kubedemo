
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System.Text;
using PriceGenerator.Models;
using Newtonsoft.Json;

namespace PriceGenerator.Services
{
    public class MessagePublisherService
    {
        private readonly string _rabbitMqUsername;
        private readonly string _rabbitMqPassword;
        private readonly string _rabbitMqHostname;
        private readonly string _rabbitMqPort;
        private readonly string _rabbitMqExchangeName;

        public MessagePublisherService(IConfiguration configuration)
        {
            var authSection = configuration.GetSection("RabbitMqAuth");
            _rabbitMqUsername = authSection.GetValue<string>("username");
            _rabbitMqPassword = authSection.GetValue<string>("password");

            var connectSection = configuration.GetSection("RabbitMqConnect");
            _rabbitMqHostname = connectSection.GetValue<string>("hostname");
            _rabbitMqPort = connectSection.GetValue<string>("port");

            _rabbitMqExchangeName = configuration.GetValue<string>("RabbitMqExchangeName");
        }

        public Task PublishPriceChanges(ICollection<StockPriceChange> priceChanges)
        {
            using (var rabbitConnection = BuildConnection())
            {
                using (var channel = rabbitConnection.CreateModel())
                {
                    channel.ExchangeDeclare(
                        exchange: _rabbitMqExchangeName,
                        type: ExchangeType.Fanout);

                    Parallel.ForEach(priceChanges, (priceChange) =>
                    {
                        PublishPriceChange(priceChange, channel);
                    });
                }
            }

            return Task.CompletedTask;
        }

        void PublishPriceChange(StockPriceChange priceChange, IModel channel)
        {
            var messagePayload = JsonConvert.SerializeObject(priceChange);
            var message = Encoding.UTF8.GetBytes(messagePayload);
            channel.BasicPublish(exchange: _rabbitMqExchangeName,
                routingKey: string.Empty,
                basicProperties: null,
                body: message);

            Console.WriteLine("Successfully wrote to the exchange - " + priceChange.PublishTime.ToString("HH:mm:ss"));
        }

        IConnection BuildConnection()
        {
            var uriString = $"amqp://{_rabbitMqUsername}:{_rabbitMqPassword}@{_rabbitMqHostname}:{_rabbitMqPort}";
            Console.WriteLine(uriString);

            var factory = new ConnectionFactory();
            factory.Uri = new Uri(uriString);

            return factory.CreateConnection();
        }
    }
}