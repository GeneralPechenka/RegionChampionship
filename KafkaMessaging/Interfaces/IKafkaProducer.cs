using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaMessaging.Interfaces
{
    public interface IKafkaProducer<in TMessage> : IDisposable
    {
        Task ProduceAsync(TMessage message,string topic, CancellationToken cancellation);
    }
}
