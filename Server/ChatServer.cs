using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    /// <summary>
    /// Представляет сервер чата, который общается с клиентами используя протокол UDP.
    /// </summary>
    public class ChatServer
    {
        private static readonly Lazy<ChatServer> _instance = new Lazy<ChatServer>(() => new ChatServer());
        private List<IPEndPoint> clients = new List<IPEndPoint>();
        private List<IObserver> observers = new List<IObserver>();
        private UdpClient udpClient;

        /// <summary>
        /// Инициализирует новый экземпляр класса ChatServer.
        /// </summary>
        private ChatServer()
        {
            udpClient = new UdpClient(12345);
        }

        /// <summary>
        /// Получает единственный экземпляр ChatServer.
        /// </summary>
        public static ChatServer Instance => _instance.Value;

        /// <summary>
        /// Запускает сервер и начинает прослушивание входящих сообщений.
        /// </summary>
        /// <param name="cancellationToken">Токен для мониторинга запросов на отмену.</param>
        /// <returns>Задача, представляющая асинхронную операцию прослушивания сервера.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Сервер ждет сообщение от клиента");
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await udpClient.ReceiveAsync();
                    if (result.Buffer == null || result.Buffer.Length == 0) break;

                    var messageText = Encoding.UTF8.GetString(result.Buffer);
                    Message? message = Message.DeserializeFromJson(messageText);
                    message.Print();
                    ICommand sendMessage = new SendMessageCommand(ChatServer.Instance, message);
                    sendMessage.Execute();
                    byte[] acknowledgment = Encoding.UTF8.GetBytes("Message received!");
                    await udpClient.SendAsync(acknowledgment, acknowledgment.Length, result.RemoteEndPoint);
                    Console.WriteLine("Подтверждение отправлено клиенту.");
                    await Task.Delay(100, cancellationToken); // Задержка для освобождения процессора
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Работа сервера остановлена по запросу.");
            }
        }

        /// <summary>
        /// Присоединяет наблюдателя к серверу для получения уведомлений о новых сообщениях.
        /// </summary>
        /// <param name="observer">Наблюдатель для присоединения.</param>
        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Отсоединяет наблюдателя от сервера, чтобы он больше не получал уведомлений.
        /// </summary>
        /// <param name="observer">Наблюдатель для отсоединения.</param>
        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        /// <summary>
        /// Регистрирует конечную точку клиента на сервере для возможности отправки сообщений этому клиенту.
        /// </summary>
        /// <param name="clientEndPoint">Конечная точка IPEndPoint клиента для регистрации.</param>
        public void RegisterClient(IPEndPoint clientEndPoint)
        {
            clients.Add(clientEndPoint);
        }

        /// <summary>
        /// Удаляет регистрацию конечной точки клиента с сервера, чтобы прекратить отправку сообщений этому клиенту.
        /// </summary>
        /// <param name="clientEndPoint">Конечная точка IPEndPoint клиента для удаления.</param>
        public void UnregisterClient(IPEndPoint clientEndPoint)
        {
            clients.Remove(clientEndPoint);
        }

        /// <summary>
        /// Уведомляет всех зарегистрированных клиентов о новом сообщении, кроме отправителя.
        /// </summary>
        /// <param name="message">Сообщение, которое будет отправлено всем клиентам.</param>
        /// <returns>Задача, представляющая асинхронную операцию уведомления.</returns>
        protected async Task Notify(Message message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message.SerializeMessageToJson());

            foreach (var clientEndPoint in clients)
            {
                if (!clientEndPoint.Equals(message.SenderEndPoint))
                {
                    await udpClient.SendAsync(messageBytes, messageBytes.Length, clientEndPoint);
                }
            }
        }

        /// <summary>
        /// Отправляет сообщение всем зарегистрированным клиентам и уведомляет всех наблюдателей.
        /// </summary>
        /// <param name="message">Сообщение для отправки.</param>
        /// <returns>Задача, представляющая асинхронную операцию отправки.</returns>
        public async Task Send(Message message)
        {
            Console.WriteLine($"Отправка сообщения: {message.Text}");

            await Notify(message);
        }
    }
}
