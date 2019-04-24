
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using KafkaNet.Protocol;
using System;
using KafkaNet.Model;
using KafkaNet;

namespace PriceGenerator.Services
{
    public class MessagePublisherService
    {
        public async Task PublishPriceChanges(IDictionary<string, decimal> stockPriceData)
        {
            await Task.WhenAll(
                stockPriceData.Select(x => PublishPriceChange(x.Key, x.Value))
            );
        }

        async Task PublishPriceChange(string symbol, decimal value)
        {
            var payload = new JObject(
                new JProperty("symbol", symbol),
                new JProperty("newPrice", value.ToString())
            );

            var messagePayload = new Message(payload.ToString());
            var kafkaUrl = new Uri("http://localhost:9092");
            var options = new KafkaOptions(kafkaUrl);

            var router = new BrokerRouter(options);
            var client = new Producer(router);
            var rawResponse = await client.SendMessageAsync("PriceChange", new[] { messagePayload });
            return;
        }
    }
}