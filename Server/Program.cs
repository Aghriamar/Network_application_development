using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        /// <summary>
        /// Входная точка программы. Асинхронно запускает метод Server с аргументом "Hello".
        /// </summary>
        static async Task Main(string[] args)
        {
            await Server("Hello");
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
        /// В случае ввода символа 'q' в консоли завершает работу сервера.
        /// </summary>
        /// <param name="name">Просто строка для инициализации объекта сообщения для тестирования.</param>
        public static async Task Server(string name)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task serverTask = Task.Run(async () =>
            {
                UdpClient udpClient = new UdpClient(12345);
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                Console.WriteLine("Сервер ждет сообщение от клиента");

                while (!cts.Token.IsCancellationRequested)
                {
                    byte[] buffer = udpClient.Receive(ref clientEndPoint);

                    if (buffer == null) break;

                    var messageText = Encoding.UTF8.GetString(buffer);
                    Message message = Message.DeserializeFromJson(messageText);
                    message.Print();
                    byte[] acknowledgment = Encoding.UTF8.GetBytes("Message received!");
                    udpClient.Send(acknowledgment, acknowledgment.Length, clientEndPoint);
                    Console.WriteLine("Подтверждение отправлено клиенту.");
                    await Task.Delay(100); // Задержка для освобождения процессора
                }
            });
            Task readKeyTask = Task.Run(() =>
            {
                while (true)
                {
                    if (Console.ReadKey(true).KeyChar == 'q')
                    {
                        cts.Cancel(); // Отмена задачи сервера
                        break;
                    }
                }
            });

            // Ожидание завершения задачи сервера
            await Task.WhenAny(serverTask, readKeyTask);
        }
    }
}