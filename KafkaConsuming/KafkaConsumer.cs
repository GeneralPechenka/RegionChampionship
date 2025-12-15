using Confluent.Kafka;
using KafkaConsuming.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KafkaConsuming
{
    public class KafkaConsumer<TMessage> : BackgroundService
    {
        private readonly string _topic;
        private readonly IConsumer<string, TMessage> _consumer;
        private readonly IMessageHandler<TMessage> _messageHandler;

        public KafkaConsumer(IOptions<KafkaConsumerSettings> options, IMessageHandler<TMessage> messageHandler)
        {
            _messageHandler = messageHandler;
            var config = new ConsumerConfig 
            {
                BootstrapServers = options.Value.BootstrapServers,
                GroupId = options.Value.GroupId
            };
            _topic = options.Value.Topic;

            _consumer = new ConsumerBuilder<string, TMessage>(config)
                  .SetValueDeserializer(new KafkaValueDeserializer<TMessage>())
                  .Build();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeAsync(stoppingToken),stoppingToken);
        }
        //public override Task StopAsync(CancellationToken cancellationToken)
        //{
        //    _consumer.Close();
        //}
        private async Task? ConsumeAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);
            try
            {
                while(!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);
                    await _messageHandler.HandleAsync(result.Message.Value, stoppingToken);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
