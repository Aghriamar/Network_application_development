using NetMQUtilities;

namespace Server
{
    internal class Program
    {
        /// <summary>
        /// Входная точка программы. Асинхронно запускает метод Server с аргументом "Hello".
        /// </summary>
        static async Task Main(string[] args)
        {
            var serverAddress = "tcp://127.0.0.1:12345";
            var server = new NetMQMessageSource(serverAddress, "Server");

            using CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task serverTask = Task.Run(() => server.StartReceiving(token));
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
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Работа сервера остановлена по запросу.");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка. {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Работа сервера остановлена по запросу.");
                cts.Cancel();
            }
        }
    }
}