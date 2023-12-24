namespace NetMQUtilities
{
    /// <summary>
    /// Определяет контракт для источника сообщений, который позволяет отправлять, принимать и обрабатывать сообщения.
    /// </summary>
    public interface IMessageSource
    {
        /// <summary>
        /// Отправляет сообщение через источник сообщений.
        /// </summary>
        /// <param name="message">Текст отправляемого сообщения.</param>
        void SendMessage(string message);

        /// <summary>
        /// Получает и обрабатывает входящее сообщение от источника.
        /// </summary>
        void ReceivedMessage();

        /// <summary>
        /// Начинает процесс приема сообщений с использованием указанного токена отмены операции.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        void StartReceiving(CancellationToken cancellationToken);

        /// <summary>
        /// Добавляет непрочитанное сообщение для определенного клиента.
        /// </summary>
        /// <param name="client">Идентификатор клиента.</param>
        /// <param name="message">Добавляемое сообщение.</param>
        void AddUnreadMessage(string client, Message message);

        /// <summary>
        /// Возвращает список непрочитанных сообщений для указанного клиента.
        /// </summary>
        /// <param name="client">Идентификатор клиента.</param>
        /// <returns>Список непрочитанных сообщений для клиента.</returns>
        List<Message> GetUnreadMessages(string client);

        /// <summary>
        /// Отправляет непрочитанные сообщения указанному клиенту.
        /// </summary>
        /// <param name="client">Идентификатор клиента.</param>
        void SendUnreadMessages(string client);
    }
}