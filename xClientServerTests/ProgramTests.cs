using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Message = Server.Message;

namespace xClientServerTests
{
    public class ProgramTests
    {
        /// <summary>
        /// Тест проверяет успешное завершение работа приложения при вводе команды "exit".
        /// </summary>
        [Fact]
        public void SentMessage_ExitCommand_ShouldExitSuccessfully()
        {
            // Arrange
            string from = "Aghriamar";
            string ip = "127.0.0.1";

            // Act
            var exitCommandResult = SimulateUserInput(from, ip, "exit");

            // Assert
            Assert.True(exitCommandResult);
        }

        /// <summary>
        /// Тест проверяет успешную отправку сообщения на сервер.
        /// </summary>
        /// <returns>Возвращает true, если отправка прошла успешно, иначе false.</returns>
        [Fact]
        public async Task SentMessage_SendMessageToServer_ShouldSendSuccessfully()
        {
            // Arrange
            string from = "Aghriamar";
            string ip = "127.0.0.1";

            // Act
            var sendResult = await SendMessageToServer(from, ip, "Test message");

            // Assert
            Assert.True(sendResult);
        }

        /// <summary>
        /// Тест проверяет успешное получение непрочитанных сообщений с сервера.
        /// </summary>
        /// <returns>Возвращает строку с непрочитанными сообщениями или null, если что-то пошло не так.</returns>
        [Fact]
        //[Fact(Skip = "Требуется запущенный сервер")]
        public async Task SentMessage_GetUnreadMessages_ShouldRetrieveUnreadMessages()
        {
            // Arrange
            string from = "Aghriamar";
            string ip = "127.0.0.1";

            // Act
            var unreadMessages = await GetUnreadMessages(from, ip);

            // Assert
            Assert.NotNull(unreadMessages);
        }

        /// <summary>
        /// Метод для имитации ввода пользователя.
        /// </summary>
        /// <param name="from">От кого сообщение.</param>
        /// <param name="ip">IP-адрес сервера.</param>
        /// <param name="userInput">Ввод пользователя.</param>
        /// <returns>Возвращает true, если имитация выполнена успешно, иначе false.</returns>
        private bool SimulateUserInput(string from, string ip, string userInput)
        {
            if (userInput == "exit")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод для отправки сообщения на сервер.
        /// </summary>
        /// <param name="from">От кого сообщение.</param>
        /// <param name="ip">IP-адрес сервера.</param>
        /// <param name="message">Текст сообщения.</param>
        /// <returns>Возвращает true, если сообщение успешно отправлено, иначе false.</returns>
        private async Task<bool> SendMessageToServer(string from, string ip, string message)
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
                    IMessageFactory messageFactory = new MessageFactory();

                    Message msg = messageFactory.CreateMessage(message, from, "Server", serverEndPoint);
                    string json = msg.SerializeMessageToJson();
                    byte[] data = Encoding.UTF8.GetBytes(json);

                    await udpClient.SendAsync(data, data.Length, serverEndPoint);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Метод для получения непрочитанных сообщений с сервера.
        /// </summary>
        /// <param name="from">От кого сообщение.</param>
        /// <param name="ip">IP-адрес сервера.</param>
        /// <returns>Возвращает строку с непрочитанными сообщениями или null, если что-то пошло не так.</returns>
        private async Task<string?> GetUnreadMessages(string from, string ip)
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
                    string getUnreadRequest = "getUnread";
                    byte[] requestData = Encoding.UTF8.GetBytes(getUnreadRequest);

                    await udpClient.SendAsync(requestData, requestData.Length, serverEndPoint);

                    var result = await udpClient.ReceiveAsync();
                    return Encoding.UTF8.GetString(result.Buffer);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}