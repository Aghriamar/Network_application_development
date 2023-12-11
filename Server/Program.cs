

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
            Task serverTask = Task.Run(() => ChatServer.Instance.StartAsync(cts.Token));
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
    }
}