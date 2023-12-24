using NetMQ;
using NetMQ.Sockets;
using System.Text.Json;

namespace NetMQUtilities
{
    /// <summary>
    /// Представляет клиента для взаимодействия с сервером сообщений при использовании NetMQ.
    /// </summary>
    public class NetMQMessageSourceClient : IMessageSourceClient
    {
        private RequestSocket _requestSocket;
        private readonly string? _serverAddress;
        private readonly string? _clientId;

        /// <summary>
        /// Инициализирует новый экземпляр класса NetMQMessageSourceClient с указанным адресом сервера и идентификатором клиента.
        /// </summary>
        /// <param name="serverAddress">Адрес сервера, к которому подключается клиент.</param>
        /// <param name="clientId">Идентификатор клиента, используемый для обмена сообщениями.</param>
        public NetMQMessageSourceClient(string? serverAddress, string? clientId)
        {
            _serverAddress = serverAddress;
            _clientId = clientId;
        }

        /// <summary>
        /// Подключает клиента к серверу сообщений.
        /// </summary>
        public void Connect()
        {
            _requestSocket.Connect(_serverAddress);
        }

        /// <summary>
        /// Отключает клиента от сервера сообщений.
        /// </summary>
        public void Disconnect()
        {
            _requestSocket.Disconnect(_serverAddress);
        }

        /// <summary>
        /// Отправляет сообщение на сервер.
        /// </summary>
        /// <param name="content">Содержимое сообщения, отправляемого на сервер.</param>
        public void SendMessage(string? content)
        {
            Message message = new Message();
            Message msg = message.CreateMessage(content, _clientId, "Server");
            string json = msg.SerializeMessageToJson();
            _requestSocket.SendFrame($"{json}");
        }

        /// <summary>
        /// Получает и выводит на консоль полученное сообщение от сервера.
        /// </summary>
        /// <returns>Полученное сообщение от сервера.</returns>
        public string ReceivedMessage()
        {
            string receivedMessage = _requestSocket.ReceiveFrameString();
            Message? message = Message.DeserializeFromJson(receivedMessage);
            message.Print();
            return receivedMessage;
        }

        /// <summary>
        /// Получает непрочитанное сообщение от сервера.
        /// </summary>
        /// <returns>Текст непрочитанного сообщения.</returns>
        public string ReceivedUnread()
        {
            string receivedMessage = _requestSocket.ReceiveFrameString();
            Message? message = Message.DeserializeFromJson(receivedMessage);
            return message.Text;
        }

        /// <summary>
        /// Начинает процесс взаимодействия с сервером: отправляет сообщения и запрашивает непрочитанные сообщения.
        /// </summary>
        public void StartReceiving()
        {
            using (var _socket = new RequestSocket())
            {
                _requestSocket = _socket;
                Connect();
                Console.WriteLine("Введите команду ('exit' для выхода, 'getUnread' для получения непрочитанных сообщений):");
                while (true)
                {
                    Console.Write("Введите сообщение для отправки серверу: ");
                    string? messageToSend = Console.ReadLine();
                    if (messageToSend.ToLower() == "exit")
                    {
                        Disconnect();
                        break;
                    }
                    if (messageToSend.ToLower() == "getunread")
                    {
                        SendMessage(messageToSend);
                        List<Message> unreadMessages = JsonSerializer.Deserialize<List<Message>>(ReceivedUnread());
                        if (unreadMessages.Count == 0)
                        {
                            Console.WriteLine("Нет непрочитанных сообщений.");
                        }
                        else
                        {
                            Console.WriteLine("Непрочитанные сообщения:");
                            foreach (var msg in unreadMessages)
                            {
                                Console.WriteLine($"Время: {msg.DateTime}\nОт: {msg.NicknameFrom}\nСообщение: {msg.Text}");
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(messageToSend))
                    {
                        SendMessage(messageToSend);
                        Console.WriteLine($"Отправлено сообщение серверу: {messageToSend}");
                        ReceivedMessage();
                    }
                }
            }
        }
    }
}
