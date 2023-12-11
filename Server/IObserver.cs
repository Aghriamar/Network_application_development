namespace Server
{
    /// <summary>
    /// Определяет интерфейс наблюдателя, который получает уведомления об изменениях.
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// Обновляет состояние наблюдателя на основе полученного сообщения.
        /// </summary>
        /// <param name="message">Сообщение, содержащее информацию об изменениях.</param>
        void Update(Message message);
    }
}
