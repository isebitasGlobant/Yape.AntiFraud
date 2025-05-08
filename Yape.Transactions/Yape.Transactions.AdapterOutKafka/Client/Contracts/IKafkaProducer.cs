using System.Threading.Tasks;

namespace Yape.Transactions.AdapterOutKafka.Client.Contracts
{
    public interface IKafkaProducer
    {
        Task SendMessageAsync(string key, object message);
    }
}
