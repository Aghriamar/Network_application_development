using NetMQUtilities;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите свой никнейм: ");
            string? nickname = Console.ReadLine();
            string[] message = new string[] { $"{nickname}", "tcp://127.0.0.1:12345" };
            args = message;
            SentMessage(args[0], args[1]);
        }

        /// <summary>
        /// Отправляет сообщения на сервер и выводит подтверждение о его получении.
        /// </summary>
        /// <param name="from">Никнейм отправителя сообщения.</param>
        /// <param name="serverAddress">IP-адрес сервера.</param>
        public static void SentMessage(string from, string serverAddress)
        {
            IMessageSourceClient messageSourceClient = new NetMQMessageSourceClient(serverAddress, from);
            messageSourceClient.StartReceiving();
        }
    }
}