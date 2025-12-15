using Confluent.Kafka;
using KafkaMessaging.Interfaces;


namespace KafkaMessaging
{
    public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
    {
        private readonly IProducer<string, TMessage> _producer;

        public KafkaProducer()
        {
            var config = new ProducerConfig 
            {
                //Это порт подключения к Kafka. 9092 - это порт по умолчанию, но можно поставить и другой
                BootstrapServers = "localhost:9092",
            };
            //KafkaJsonSerializer - это класс, чтобы для перевода из TMessage в JSON
            _producer = new ProducerBuilder<string, TMessage>(config)
                .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
                .Build();
        }
        //Метод для очистки ресурсов после завершения работы producer
        public void Dispose()
        {
            _producer?.Dispose();
        }

        public async Task ProduceAsync(TMessage message, string topic, CancellationToken cancellation)
        {
           
            await _producer.ProduceAsync(topic, new Message<string, TMessage>
            { 
                Key="uniq1",
                Value=message  
            },
            cancellation);
        }
    }
}
