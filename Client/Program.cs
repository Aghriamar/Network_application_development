using Server;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

                while (true)
                {
                    Console.WriteLine("Введите сообщение.");
                    string messageText = Console.ReadLine();

                    // Проверка на команду "Exit" для завершения работы клиента
                    if (messageText?.ToLower() == "exit")
                        break;

                    if (!string.IsNullOrEmpty(messageText))
                    {
                        Message message = messageFactory.CreateMessage(messageText, from, "Server", serverEndPoint);
                        string json = message.SerializeMessageToJson();
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        udpClient.Send(data, data.Length, serverEndPoint);
                        byte[] receivedData = udpClient.Receive(ref serverEndPoint);
                        string receivedMessage = Encoding.UTF8.GetString(receivedData);
                        Console.WriteLine($"Сообщение получено сервером: {receivedMessage}");
                    }
                }
            }
        }
    }
}