using System;
using System.Threading.Tasks;

namespace Goober.RabbitMq.Producer.Abstractions
{
    /// <summary>
    /// Предоставляет абстракцию Издателя в модели подписчик/издатель (Publisher/Subscriber)
    /// </summary>
    public interface IEventProducer : IDisposable
    {
        /// <summary>
        /// Гарантировано передаёт сообщение брокеру сообщений, неблокирующий вызов.
        /// </summary>
        /// <typeparam name="T">Тип сообщения</typeparam>
        /// <param name="event">Сообщение</param>        
        Task PublishAsync<T>(T @event) where T : class;
    }
}
