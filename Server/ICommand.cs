namespace Server
{
    /// <summary>
    /// Представляет команду, которая выполняет определённое действие.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Выполняет действие, ассоциированное с командой.
        /// </summary>
        void Execute();
    }
}
