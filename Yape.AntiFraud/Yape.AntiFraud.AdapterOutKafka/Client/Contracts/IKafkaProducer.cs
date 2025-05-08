using System.Threading.Tasks;

namespace Yape.AntiFraud.AdapterOutKafka.Client.Contracts
{
    public interface IKafkaProducer
    {
        Task SendMessageAsync(string key, object message);
    }
}
