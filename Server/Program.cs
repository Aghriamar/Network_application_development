using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server("Hello");
        }

        /// <summary>
        /// Тестовый метод для сериализации и десериализации объекта сообщения для проверки функциональности.
        /// </summary>
        public void task1()
        {
            Message msg = new Message() { Text = "Hello", DateTime = DateTime.Now, NicknameFrom = "Aghriamar", NicknameTo = "All" };
            string json = msg.SerializeMessageToJson();
            Console.WriteLine(json);
            Message? msgDeserialized = Message.DeserializeFromJson(json);
        }

        /// <summary>
        /// Ожидает сообщения от клиента, выводит полученные сообщения и отправляет подтверждение клиенту.
        /// </summary>
        /// <param name="name">Просто строка для инициализации объекта сообщения для тестирования.</param>
        public static void Server(string name)
        {
            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Сервер ждет сообщение от клиента");

            while (true)
            {
                byte[] buffer = udpClient.Receive(ref clientEndPoint);

                if (buffer == null) break;

                var messageText = Encoding.UTF8.GetString(buffer);
                Message message = Message.DeserializeFromJson(messageText);
                message.Print();
                byte[] acknowledgment = Encoding.UTF8.GetBytes("Message received!");
                udpClient.Send(acknowledgment, acknowledgment.Length, clientEndPoint);
                Console.WriteLine("Подтверждение отправлено клиенту.");
            }
        }
    }
}