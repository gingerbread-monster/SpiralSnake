using System;

namespace SpiralSnake
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int fieldSize = 12;

            _ = new SpiralSnake(fieldSize).Run();

            Console.ReadKey();
        }
    }
}
