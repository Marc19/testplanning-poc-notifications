using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotificationServer.EventHandlers;
using NotificationServer.Events;

namespace NotificationServer.Consumers
{
    public class MyKafkaConsumer : BackgroundService
    {
        ConsumerConfig _consumerConfig;
        private readonly IExperimentEventsHandler _experimentEventsHandler;
        private readonly IConfiguration Configuration;
        private readonly string EXPERIMENTS_TOPIC;
        private readonly string METHODS_TOPIC;

        public MyKafkaConsumer(ConsumerConfig consumerConfig, IExperimentEventsHandler experimentEventsHandler, IConfiguration configuration)
        {
            _consumerConfig = new ConsumerConfig
            {
                GroupId = "notifications-consumer",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _experimentEventsHandler = experimentEventsHandler;
            Configuration = configuration;
            EXPERIMENTS_TOPIC = Configuration["ExperimentsTopic"];
            METHODS_TOPIC = Configuration["MethodsTopic"];
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsumer(stoppingToken));
            return Task.CompletedTask;
        }

        private void StartConsumer(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var c = new ConsumerBuilder<string, string>(_consumerConfig).Build())
                {
                    c.Subscribe(new List<string>() { EXPERIMENTS_TOPIC, METHODS_TOPIC });

                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, e) => {
                        e.Cancel = true; // prevent the process from terminating.
                        cts.Cancel();
                    };

                    try
                    {
                        while (true)
                        {
                            Console.Write("Starting loop");
                            try
                            {
                                var cr = c.Consume(cts.Token);
                                var msg = cr.Message;
                                //var key = cr.Message.Key;
                                var val = cr.Message.Value;
                                Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");

                                Event @event = GetEventType(val);
                                if (@event != null)
                                    _experimentEventsHandler.Handle(@event);
                                //otherwise, it's not a command
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Error occured: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        c.Close();
                    }
                }
            }
        }

        private Event GetEventType(string message)
        {
            JObject rss = JObject.Parse(message);
            string messageType = (string)rss["messageType"];
            string payload = ((JObject)rss["payload"]).ToString();

            return messageType switch
            {
                "ExperimentCreated" => JsonConvert.DeserializeObject<ExperimentCreated>(payload),
                "ExperimentCreationFailed" => JsonConvert.DeserializeObject<ExperimentCreationFailed>(payload),
                "ExperimentDeleted" => JsonConvert.DeserializeObject<ExperimentDeleted>(payload),
                "ExperimentDeletionFailed" => JsonConvert.DeserializeObject<ExperimentDeletionFailed>(payload),
                "ExperimentUpdated" => JsonConvert.DeserializeObject<ExperimentUpdated>(payload),
                "ExperimentUpdateFailed" => JsonConvert.DeserializeObject<ExperimentUpdateFailed>(payload),
                "ExperimentCreatedWithMethods" => JsonConvert.DeserializeObject<ExperimentCreatedWithMethods>(payload),
                "ExperimentWithMethodsCreationFailed" => JsonConvert.DeserializeObject<ExperimentWithMethodsCreationFailed>(payload),
                "MethodCreated" => JsonConvert.DeserializeObject<MethodCreated>(payload),
                "MethodCreationFailed" => JsonConvert.DeserializeObject<MethodCreationFailed>(payload),
                _ => null,
            };
        }
    }
}
