namespace NetMQUtilities
{
    /// <summary>
    /// Определяет контракт для клиента источника сообщений, позволяющего подключаться, отправлять и получать сообщения.
    /// </summary>
    public interface IMessageSourceClient
    {
        /// <summary>
        /// Устанавливает соединение с источником сообщений.
        /// </summary>
        void Connect();

        /// <summary>
        /// Отправляет сообщение через клиент источника сообщений.
        /// </summary>
        /// <param name="message">Текст отправляемого сообщения.</param>
        void SendMessage(string message);

        /// <summary>
        /// Получает входящее сообщение от источника.
        /// </summary>
        /// <returns>Текст принятого сообщения.</returns>
        string ReceivedMessage();

        /// <summary>
        /// Начинает процесс приема сообщений от источника.
        /// </summary>
        void StartReceiving();
    }
}
