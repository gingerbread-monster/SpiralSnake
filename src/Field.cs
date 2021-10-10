using System;

namespace SpiralSnake
{
    class Field
    {
        public int[,] Matrix;

        public int MatrixSize { get; }

        public Field(int matrixSize)
        {
            MatrixSize = matrixSize;
            Matrix = new int [MatrixSize, MatrixSize];
        }

        public bool PositionIsWithinRange(int rowIndex, int columnIndex)
        {
            return (rowIndex >= 0 && rowIndex < MatrixSize)
                && (columnIndex >= 0 && columnIndex < MatrixSize);
        }

        public bool PositionIsFree(int rowIndex, int columnIndex)
        {
            if (!PositionIsWithinRange(rowIndex, columnIndex))
                throw new IndexOutOfRangeException();

            return Matrix[rowIndex, columnIndex] == 0;
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void Reset()
        {
            Matrix.Initialize();
        }

        public void Print()
        {
            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    Console.Write(Matrix[i,j] + " ");
                }

                Console.WriteLine();
            }
        }
    }
}