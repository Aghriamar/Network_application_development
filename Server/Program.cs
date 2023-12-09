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
            using CancellationTokenSource cts = new CancellationTokenSource();

            Task serverTask = Task.Run(() => Server("Hello", cts.Token));
            Task readKeyTask = Task.Run(() =>
            {
                Console.WriteLine("Для завершения работы сервера нажмите клавишу 'q' и Enter.");
                while (true)
                {
                    if (Console.ReadKey(true).KeyChar == 'q')
                    {
                        cts.Cancel();
                        break;
                    }
                }
            });

            try
            {
                await Task.WhenAny(serverTask, readKeyTask);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Работа сервера остановлена по запросу.");
            }
            finally
            {
                Console.WriteLine("Работа сервера остановлена по запросу.");
                cts.Cancel(); // Убедимся, что сервер завершит работу
            }
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
        public static async Task Server(string name, CancellationToken cancellationToken)
        {
            try
            {
                UdpClient udpClient = new UdpClient(12345);
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                Console.WriteLine("Сервер ждет сообщение от клиента");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await udpClient.ReceiveAsync();
                    if (result.Buffer == null || result.Buffer.Length == 0) break;

                    var messageText = Encoding.UTF8.GetString(result.Buffer);
                    Message message = Message.DeserializeFromJson(messageText);
                    message.Print();
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
    }
}