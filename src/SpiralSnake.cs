using System;
using System.Threading.Tasks;
using SpiralSnake.Exceptions;

namespace SpiralSnake
{
    class SpiralSnake
    {
        Field _field;

        MoveDirection _moveDirection;

        int _headPositionRow;
        int _headPositionColumn;

        int _delay;

        int _snakeLengthCounter;

        public SpiralSnake(
            int fieldSize,
            int delayBetweenMovement = 200)
        {
            if (fieldSize < 5)
                throw new ArgumentException("Invalid field size.");

            _field = new Field(fieldSize);

            _delay = delayBetweenMovement;

            Reset();
        }

        public async Task Run()
        {
            try
            {
                await Loop();
            }
            catch (Exception exception)
            {
                Console.WriteLine();
                Console.WriteLine(exception.Message);
                Console.WriteLine($"Snake length: {_snakeLengthCounter}");
            }
        }

        async Task Loop()
        {
            while (true)
            {
                _field.Clear();

                _field.Print();

                await Task.Delay(_delay);

                Move();
            }
        }

        void Move()
        {
            if (!TryMove())
            {
                if (!TryChangeDirection())
                {
                    throw new DeadEndException();
                }

                _moveDirection = GetNewDirection(_moveDirection);
            }

            var offsets = TranslateDirectionToOffset(_moveDirection);

            _headPositionRow += offsets.rowOffset;
            _headPositionColumn += offsets.columnOffset;

            _field.Occupy(_headPositionRow, _headPositionColumn);

            _snakeLengthCounter++;
        }

        bool TryMove(int? rowOffset = null, int? columnOffset = null)
        {
            var offsets = TranslateDirectionToOffset(_moveDirection);

            if (rowOffset != null && columnOffset != null)
            {
                offsets.rowOffset = rowOffset.Value;
                offsets.columnOffset = columnOffset.Value;
            }

            var nextPositionRow = _headPositionRow + offsets.rowOffset;
            var nextPositionColumn = _headPositionColumn + offsets.columnOffset;

            if (!_field.PositionIsWithinRange(nextPositionRow, nextPositionColumn))
                return false;

            var secondNextRow = _headPositionRow + offsets.rowOffset * 2;
            var secondNextColumn = _headPositionColumn + offsets.columnOffset * 2;
            if (_field.PositionIsWithinRange(secondNextRow, secondNextColumn))
            {
                if (!_field.PositionIsFree(secondNextRow, secondNextColumn))
                {
                    return false;
                }
            }

            return true;
        }

        bool TryChangeDirection()
        {
            var newMoveDirection = GetNewDirection(_moveDirection);

            var newOffsets = TranslateDirectionToOffset(newMoveDirection);

            var canMoveForward = TryMove(newOffsets.rowOffset, newOffsets.columnOffset);

            if (_field.MatrixSize % 2 == 0 && canMoveForward) // check for dead end on matrix with even number size
            {
                var nextPositionRow = _headPositionRow + newOffsets.rowOffset;
                var nextPositionColumn = _headPositionColumn + newOffsets.columnOffset;

                var currentOffsets = TranslateDirectionToOffset(_moveDirection);

                var reversedCurrentRowOffset = -currentOffsets.rowOffset;
                var reversedCurrentColumnOffset = -currentOffsets.columnOffset;

                var rightDiagonalCellIsOccupied = !_field.PositionIsFree(
                    nextPositionRow + reversedCurrentRowOffset,
                    nextPositionColumn + reversedCurrentColumnOffset);

                canMoveForward = !rightDiagonalCellIsOccupied;
            }

            return canMoveForward;
        }

        static (int rowOffset, int columnOffset) TranslateDirectionToOffset(MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Right => (0, 1),
                MoveDirection.Down => (1, 0),
                MoveDirection.Left => (0, -1),
                MoveDirection.Up => (-1, 0),
                _ => throw new ArgumentException(nameof(direction))
            };
        }

        static MoveDirection GetNewDirection(MoveDirection currentDirection)
        {
            // spiral movement only
            return currentDirection switch
            {
                MoveDirection.Right => MoveDirection.Down,
                MoveDirection.Down => MoveDirection.Left,
                MoveDirection.Left => MoveDirection.Up,
                MoveDirection.Up => MoveDirection.Right,
                _ => throw new ArgumentException(nameof(currentDirection))
            };
        }

        void Reset()
        {
            _headPositionRow = _headPositionColumn = 0; // snake head starting position

            _field.Reset();

            _field.Occupy(_headPositionRow, _headPositionColumn);

            _moveDirection = MoveDirection.Right;

            _snakeLengthCounter = 1;
        }

        enum MoveDirection
        {
            Left,
            Right,
            Down,
            Up
        }
    }
}