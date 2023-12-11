namespace Server
{
    /// <summary>
    /// Команда для отправки сообщения через чат-сервер.
    /// </summary>
    public class SendMessageCommand : ICommand
    {
        private readonly ChatServer _server;
        private readonly Message _message;

        /// <summary>
        /// Инициализирует новый экземпляр команды для отправки сообщения с ссылками на сервер и сообщение.
        /// </summary>
        /// <param name="server">Экземпляр сервера, через который будет происходить отправка сообщения.</param>
        /// <param name="message">Сообщение, которое необходимо отправить.</param>
        public SendMessageCommand(ChatServer server, Message message)
        {
            _server = server;
            _message = message;
        }

        /// <summary>
        /// Выполняет асинхронную операцию отправки сообщения через чат-сервер.
        /// </summary>
        public async void Execute()
        {
            await _server.Send(_message);
        }
    }
}
