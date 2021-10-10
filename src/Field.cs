using System;

namespace SpiralSnake
{
    class Field
    {
        const char freePosition = '⬛';
        const char occupiedPosition = '⬜';

        public char[,] Matrix;

        public int MatrixSize { get; }

        public Field(int matrixSize)
        {
            MatrixSize = matrixSize;
            Matrix = new char [MatrixSize, MatrixSize];
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

            return Matrix[rowIndex, columnIndex] == freePosition;
        }

        public void Occupy(int rowIndex, int columnIndex)
        {
            if (!PositionIsWithinRange(rowIndex, columnIndex))
                throw new IndexOutOfRangeException();

            Matrix[rowIndex, columnIndex] = occupiedPosition;
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void Reset()
        {
            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    Matrix[i, j] = freePosition;
                }
            }
        }

        public void Print()
        {
            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    Console.Write(Matrix[i,j]);
                }

                Console.WriteLine();
            }
        }
    }
}