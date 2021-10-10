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

        int _snakeLength;

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
                Console.WriteLine($"Snake length: {_snakeLength}");
            }
        }

        async Task Loop()
        {
            while (true)
            {
                _field.Clear();

                _field.Print();

                await Task.Delay(_delay);

                MoveV2();
            }
        }

        void MoveV2()
        {
            if (!TryMoveV2())
            {
                if (!TryChangeDirectionV2())
                {
                    throw new DeadEndException();
                }

                _moveDirection = GetNewDirection(_moveDirection);
            }

            var offsets = TranslateDirectionToOffset(_moveDirection);

            _headPositionRow += offsets.rowOffset;
            _headPositionColumn += offsets.columnOffset;

            _field.Occupy(_headPositionRow, _headPositionColumn);

            _snakeLength++;
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

            switch (_moveDirection)
            {
                case MoveDirection.Right:
                    _headPositionColumn++; 
                    break;
                case MoveDirection.Down:
                    _headPositionRow++; 
                    break;
                case MoveDirection.Left:
                    _headPositionColumn--; 
                    break;
                case MoveDirection.Up:
                    _headPositionRow--; 
                    break;
            }

            _field.Matrix[_headPositionRow, _headPositionColumn] = 1;
        }

        bool TryMoveV2(int? rowOffset = null, int? columnOffset = null)
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

        bool TryMove()
        {
            bool withinRange;

            if (_moveDirection == MoveDirection.Right)
            {
                withinRange = _headPositionColumn + 1 < _field.MatrixSize; // on next cell

                if (withinRange)
                {
                    withinRange = _headPositionColumn + 2 < _field.MatrixSize; // on second next cell
                    if (withinRange)
                    {
                        return _field.Matrix[_headPositionRow, _headPositionColumn + 2] != 1; // snake's head not gonna touch the tail
                    }
                    else return true;
                }
                else return false;
            }

            if (_moveDirection == MoveDirection.Down)
            {
                withinRange = _headPositionRow + 1 < _field.MatrixSize; // on next cell

                if (withinRange)
                {
                    withinRange = _headPositionRow + 2 < _field.MatrixSize; // on second next cell
                    if (withinRange)
                    {
                        return _field.Matrix[_headPositionRow + 2, _headPositionColumn] != 1; // snake's head not gonna touch the tail
                    }
                    else return true;
                }
                else return false;
            }

            if (_moveDirection == MoveDirection.Left)
            {
                withinRange = _headPositionColumn - 1 >= 0; // on next cell

                if (withinRange)
                {
                    withinRange = _headPositionColumn - 2 >= 0; // on second next cell
                    if (withinRange)
                    {
                        return _field.Matrix[_headPositionRow, _headPositionColumn - 2] != 1; // snake's head not gonna touch the tail
                    }
                    else return true;
                }
                else return false;
            }

            if (_moveDirection == MoveDirection.Up)
            {
                withinRange = _headPositionRow - 1 >= 0; // on next cell

                if (withinRange)
                {
                    withinRange = _headPositionRow - 2 >= 0; // on second next cell
                    if (withinRange)
                    {
                        return _field.Matrix[_headPositionRow - 2, _headPositionColumn] != 1; // snake's head not gonna touch the tail
                    }
                    else return true;
                }
                else return false;
            }

            throw new InvalidOperationException("Movement direction is undefined.");
        }

        bool TryChangeDirectionV2()
        {
            var newMoveDirection = GetNewDirection(_moveDirection);

            var newOffsets = TranslateDirectionToOffset(newMoveDirection);

            var canMoveForward = TryMoveV2(newOffsets.rowOffset, newOffsets.columnOffset);

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

        bool TryChangeDirection()
        {
            int nextPositionRow;
            int nextPositionColumn;

            // check rotate is possible relatively to current movement direction
            if (_moveDirection == MoveDirection.Right)
            {
                nextPositionRow = _headPositionRow + 1;
                nextPositionColumn = _headPositionColumn;

                return _field.Matrix[nextPositionRow + 1, nextPositionColumn] != 1; // snake's head not gonna touch the tail 
            }
            if (_moveDirection == MoveDirection.Down)
            {
                nextPositionRow = _headPositionRow;
                nextPositionColumn = _headPositionColumn - 1;

                return _field.Matrix[nextPositionRow, nextPositionColumn - 1] != 1;
            }
            if (_moveDirection == MoveDirection.Left)
            {
                nextPositionRow = _headPositionRow - 1;
                nextPositionColumn = _headPositionColumn;

                return _field.Matrix[nextPositionRow - 1, nextPositionColumn] != 1;
            }
            if (_moveDirection == MoveDirection.Up)
            {
                nextPositionRow = _headPositionRow;
                nextPositionColumn = _headPositionColumn + 1;

                return _field.Matrix[nextPositionRow, nextPositionColumn + 1] != 1;
            }

            throw new InvalidOperationException("Movement direction is undefined.");
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
            _headPositionRow = _headPositionColumn = 0;

            _field.Reset();

            _field.Matrix[_headPositionRow, _headPositionColumn] = 1;

            _moveDirection = MoveDirection.Right;

            _snakeLength = 1;
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