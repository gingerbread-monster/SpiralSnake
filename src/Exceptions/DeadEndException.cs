using System;

namespace SpiralSnake.Exceptions
{
    /// <summary>
    /// Represents exception that occurs
    /// when snake's head is in dead end
    /// with no way to move forward.
    /// </summary>
    class DeadEndException : Exception
    {
        public override string Message =>
            "Snake's head is in a dead end and cannot move forward.";
    }
}