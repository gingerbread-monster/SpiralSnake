using System;

namespace SpiralSnake
{
    class Program
    {
        static void Main(string[] args)
        {
            int fieldSize = 12;

            _ = new SpiralSnake(fieldSize).Run();

            Console.ReadKey();
        }
    }
}
