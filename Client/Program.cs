using Server;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] message = new string[] { "Aghriamar", "127.0.0.1" };
            args = message;
            SentMessage(args[0], args[1]);
        }

        /// <summary>
        /// Отправляет сообщения на сервер и выводит подтверждение о его получении.
        /// </summary>
        /// <param name="from">Никнейм отправителя сообщения.</param>
        /// <param name="ip">IP-адрес сервера.</param>
        public static void SentMessage(string from, string ip)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
                IMessageFactory messageFactory = new MessageFactory();
                Console.WriteLine("Введите команду ('exit' для выхода, 'getUnread' для получения непрочитанных сообщений):");

                while (true)
                {
                    string input = Console.ReadLine();

                    if (input?.ToLower() == "exit")
                        break;

                    if (input?.ToLower() == "getunread")
                    {
                        // Запрос непрочитанных сообщений
                        string getUnreadRequest = "getUnread";
                        byte[] requestData = Encoding.UTF8.GetBytes(getUnreadRequest);
                        udpClient.Send(requestData, requestData.Length, serverEndPoint);

                        byte[] receivedData = udpClient.Receive(ref serverEndPoint);
                        string receivedMessage = Encoding.UTF8.GetString(receivedData);

                        List<Message> unreadMessages = JsonSerializer.Deserialize<List<Message>>(receivedMessage);

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
                    else if (!string.IsNullOrEmpty(input))
                    {
                        Message message = messageFactory.CreateMessage(input, from, "Server", serverEndPoint);
                        string json = message.SerializeMessageToJson();
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        udpClient.Send(data, data.Length, serverEndPoint);
                        byte[] acknowledgmentData = udpClient.Receive(ref serverEndPoint);
                        string acknowledgment = Encoding.UTF8.GetString(acknowledgmentData);
                        Console.WriteLine($"Сообщение получено сервером: {acknowledgment}");
                    }
                }
            }
        }
    }
}