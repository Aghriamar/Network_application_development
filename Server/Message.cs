using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Класс, представляющий сообщение для обмена между сервером и клиентом.
    /// </summary>
    public class Message
    {
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string NicknameFrom { get; set; }
        public string NicknameTo { get; set; }

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
            return $"{this.DateTime} получено сообщение {this.Text} от {this.NicknameFrom}";
        }
    }
}
