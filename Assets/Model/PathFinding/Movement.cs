using System;
using Model.MapModelComponents;

namespace Model.PathFinding
{
    /// <summary>
    /// Movement class describes a movement in a direction and encapsulates
    /// the cost of the movement. This abstraction is basically just the
    /// delta of the coordinates the movement represents. The cost represents
    /// the summation of weights and penalties or bonuses on the cell.
    /// </summary>
    /// <remarks>This class is immutable!</remarks>
    public class Movement
    {
        private readonly Coordinate _moveDirection;
        private readonly double _moveCost;

        /// <summary>Movement constructor</summary>
        /// <param name="moveDirection">Defines the direction of movement, 
        /// can only be a valid direction - one in any direction eg. (1,1,0)</param>
        /// <param name="moveCost">Defines the cost of movement for this move</param>
        public Movement(Coordinate moveDirection, double moveCost)
        {
            if ((Math.Abs(moveDirection.x) > 1) || (Math.Abs(moveDirection.y) > 1) || (Math.Abs(moveDirection.z) > 1) || (moveCost < 0))
            {
                throw new ArgumentException();
            }
            _moveDirection = moveDirection;
            _moveCost = moveCost * GetMoveMultiplier(moveDirection);
        }

        /// <summary>Movement Constructor -- shortcut for tests and quick initialization</summary>
        /// <param name="x">x axis of movement</param>
        /// <param name="y">y axis of movement</param>
        /// <param name="z">z axis of movement</param>
        /// <param name="cost">cost of movement</param>
        public Movement(int x, int y, int z, double cost) : this(new Coordinate(x, y, z), cost) {
        }

        /// <summary>Get the movement direction that this move represents</summary>
        /// <returns>Coordinate representing the movement delta</returns>
        public Coordinate GetMoveDirection()
        {
            return _moveDirection;
        }

        /// <summary>Get the movement cost of this move</summary>
        /// <returns>integer representing the movement cost</returns>
        public double GetMoveCost()
        {
            return _moveCost;
        }

        public double GetMoveMultiplier(Coordinate coordinate)
        {
            return Math.Sqrt(Math.Abs(coordinate.x) + Math.Abs(coordinate.y) + Math.Abs(coordinate.z));
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Movement movement = (Movement)obj;
            return (_moveDirection == movement._moveDirection) && (Math.Abs(_moveCost - movement._moveCost) < 0.0001);
        }

        public override int GetHashCode()
        {
            return (int) (_moveDirection.GetHashCode() + _moveCost);
        }
    }
}