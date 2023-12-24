using System.Text.Json;

namespace NetMQUtilities
{
    /// <summary>
    /// Класс, представляющий сообщение для обмена между сервером и клиентом.
    /// </summary>
    public class Message : IMessageFactory
    {
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Дата и время сообщения.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Псевдоним отправителя сообщения.
        /// </summary>
        public string? NicknameFrom { get; set; }

        /// <summary>
        /// Псевдоним получателя сообщения.
        /// </summary>
        public string? NicknameTo { get; set; }

        /// <summary>
        /// Сериализует сообщение в формат JSON.
        /// </summary>
        /// <returns>JSON-строку, представляющую сообщение.</returns>
        public string SerializeMessageToJson() => JsonSerializer.Serialize(this);

        /// <summary>
        /// Десериализует JSON-строку в объект сообщения.
        /// </summary>
        /// <param name="message">JSON-строка, представляющая сообщение.</param>
        /// <returns>Объект сообщения, полученный из JSON-строки.</returns>
        public static Message? DeserializeFromJson(string message) => JsonSerializer.Deserialize<Message>(message);

        /// <summary>
        /// Создает сообщение с заданными параметрами, представляющее собой данные для обмена между клиентами.
        /// </summary>
        /// <param name="content">Текст сообщения, которое будет отправлено.</param>
        /// <param name="from">Псевдоним отправителя сообщения.</param>
        /// <param name="to">Псевдоним получателя сообщения, может быть null или пустым для широковещательных сообщений.</param>
        /// <returns>Созданный экземпляр сообщения с заполненными данными.</returns>
        public Message CreateMessage(string content, string from, string to)
        {
            return new Message
            {
                Text = content,
                NicknameFrom = from,
                NicknameTo = to,
                DateTime = DateTime.Now,
            };
        }

        /// <summary>
        /// Выводит сообщение в консоль.
        /// </summary>
        public void Print()
        {
            Console.WriteLine(ToString());
        }

        /// <summary>
        /// Возвращает строковое представление объекта сообщения.
        /// </summary>
        /// <returns>Строку с датой, текстом сообщения и отправителем.</returns>
        public override string ToString()
        {
            return $"{this.DateTime} получено сообщение: {this.Text} от {this.NicknameFrom}";
        }
    }
}
