using NetMQ;
using NetMQ.Sockets;
using System.Text.Json;

namespace NetMQUtilities
{
    /// <summary>
    /// Представляет источник сообщений для обмена между сервером и клиентом с использованием NetMQ.
    /// </summary>
    public class NetMQMessageSource : IMessageSource
    {
        private ResponseSocket _responseSocket;
        private Dictionary<string, List<Message>> unreadMessages = new Dictionary<string, List<Message>>();
        private readonly string _serverAddress;
        private readonly string _clientId;
        private bool _sendUnread = false;

        /// <summary>
        /// Инициализирует новый экземпляр класса NetMQMessageSource с указанным адресом сервера и идентификатором клиента.
        /// </summary>
        /// <param name="serverAddress">Адрес сервера, к которому подключается источник сообщений.</param>
        /// <param name="clientId">Идентификатор клиента, используемый для обмена сообщениями.</param>
        public NetMQMessageSource(string serverAddress, string clientId)
        {
            _serverAddress = serverAddress;
            _clientId = clientId;
        }

        /// <summary>
        /// Отправляет сообщение на сервер.
        /// </summary>
        /// <param name="content">Содержимое сообщения, отправляемого на сервер.</param>
        public void SendMessage(string content)
        {
            Message message = new Message();
            Message msg = message.CreateMessage(content, "Server", _clientId);
            string json = msg.SerializeMessageToJson();
            _responseSocket.SendFrame($"{json}");
        }

        /// <summary>
        /// Обрабатывает полученное сообщение от клиента. Если сообщение - запрос на получение непрочитанных сообщений, отправляет их клиенту. 
        /// В противном случае, добавляет сообщение в список непрочитанных сообщений и выводит его на консоль.
        /// </summary>
        public void ReceivedMessage()
        {
            string receivedMessage = _responseSocket.ReceiveFrameString();
            Message? message = Message.DeserializeFromJson(receivedMessage);
            if (message.Text.ToLower() == "getunread")
            {
                SendUnreadMessages(message.NicknameFrom);
                _sendUnread = true;
            }
            else
            {
                AddUnreadMessage(message.NicknameFrom, message);
                message.Print();
            }
        }

        /// <summary>
        /// Начинает прослушивание входящих сообщений от клиента.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        public void StartReceiving(CancellationToken cancellationToken)
        {
            try
            {
                using (var Socket = new ResponseSocket())
                {
                    _responseSocket = Socket;
                    _responseSocket.Bind(_serverAddress);
                    Console.WriteLine("Сервер ждет сообщение от клиента");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        ReceivedMessage();
                        if (_sendUnread == false)
                        {
                            string responseMessage = $"Сообщение доставлено!";
                            SendMessage(responseMessage);
                            Console.WriteLine($"Подтверждение отправлено клиенту: {responseMessage}");
                        }
                        else
                            _sendUnread = false;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Работа сервера остановлена по запросу.");
            }
        }

        /// <summary>
        /// Добавляет новое непрочитанное сообщение от клиента в список непрочитанных сообщений.
        /// </summary>
        /// <param name="client">Идентификатор клиента.</param>
        /// <param name="message">Непрочитанное сообщение для добавления.</param>
        public void AddUnreadMessage(string client, Message message)
        {
            if (!unreadMessages.ContainsKey(client))
            {
                unreadMessages[client] = new List<Message>();
            }

            unreadMessages[client].Add(message);
        }

        /// <summary>
        /// Получает список непрочитанных сообщений для определенного клиента.
        /// </summary>
        /// <param name="client">Идентификатор клиента.</param>
        /// <returns>Список непрочитанных сообщений для указанного клиента.</returns>
        public List<Message> GetUnreadMessages(string client)
        {
            if (unreadMessages.ContainsKey(client))
            {
                return unreadMessages[client];
            }

            return new List<Message>();
        }

        /// <summary>
        /// Отправляет непрочитанные сообщения клиенту в виде сериализованного JSON.
        /// </summary>
        /// <param name="client">Идентификатор клиента.</param>
        public void SendUnreadMessages(string client)
        {
            List<Message> unreadMessages = GetUnreadMessages(client);
            string messagesJson = JsonSerializer.Serialize(unreadMessages);
            Message message = new Message();
            Message msg = message.CreateMessage(messagesJson, "Server", _clientId);
            string json = msg.SerializeMessageToJson();
            _responseSocket.SendFrame($"{json}");
        }
    }
}
