namespace NetMQUtilities
{
    /// <summary>
    /// Определяет фабрику для создания экземпляров сообщений.
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// Создает новый экземпляр сообщения с заданным содержимым и информацией об отправителе и получателе.
        /// </summary>
        /// <param name="content">Текст сообщения.</param>
        /// <param name="from">Идентификатор отправителя сообщения.</param>
        /// <param name="to">Идентификатор получателя сообщения.</param>
        /// <returns>Экземпляр сообщения, сконфигурированный согласно предоставленным параметрам.</returns>
        Message CreateMessage(string content, string from, string to);
    }
}
