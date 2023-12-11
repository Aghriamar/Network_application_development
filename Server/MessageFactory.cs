using System.Net;

namespace Server
{
    /// <summary>
    /// Фабрика для создания экземпляров сообщений, реализующая интерфейс IMessageFactory.
    /// </summary>
    public class MessageFactory : IMessageFactory
    {
        /// <summary>
        /// Создает сообщение с заданными параметрами, представляющее собой данные для обмена между клиентами.
        /// </summary>
        /// <param name="content">Текст сообщения, которое будет отправлено.</param>
        /// <param name="from">Псевдоним отправителя сообщения.</param>
        /// <param name="to">Псевдоним получателя сообщения, может быть null или пустым для широковещательных сообщений.</param>
        /// <param name="senderEndPoint">Сетевая конечная точка отправителя сообщения, содержащая IP-адрес и порт.</param>
        /// <returns>Созданный экземпляр сообщения с заполненными данными.</returns>
        public Message CreateMessage(string content, string from, string to, IPEndPoint senderEndPoint)
        {
            return new Message
            {
                Text = content,
                NicknameFrom = from,
                NicknameTo = to,
                DateTime = DateTime.Now,
                SenderEndPoint = senderEndPoint
            };
        }
    }
}
