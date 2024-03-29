﻿using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.MapModelComponents;

namespace Automate.Model.PathFinding
{
    public class MovementPath
    {
        private Dictionary<string, Movement> _movementMap;
        private List<Movement> _movements;
        public float TotalCost => (float) _movements.Sum(value => value.GetMoveCost());
        private readonly Coordinate _startCoordinate;
        private Coordinate _endCoordinate;

        public MovementPath(Coordinate startCoordinate) {
            _movementMap = new Dictionary<string, Movement>();
            _movements = new List<Movement>();
            _startCoordinate = startCoordinate;
            _endCoordinate = startCoordinate;
        }

        public MovementPath(MovementPath clone) {
            if (clone == null)
                throw new ArgumentNullException();
            _movements = new List<Movement>(clone._movements);
            _movementMap = new Dictionary<string, Movement>(clone._movementMap);
            _startCoordinate = clone._startCoordinate;
            _endCoordinate = clone._endCoordinate;
        }

        public void AddMovement(Movement newMovement)
        {
            _movementMap[_endCoordinate.ToString()] = newMovement;
            _movements.Add(newMovement);
            _endCoordinate = _endCoordinate + newMovement.GetMoveDirection();
        }

        public void RemoveLastMovement()
        {
            if (_movements.Count == 0)
                throw new PathOperationException("Cannot remove last movement, path is now empty!");
            Movement movement = _movements[_movements.Count - 1];
            _movements.RemoveAt(_movements.Count - 1);
            _endCoordinate = _endCoordinate - movement.GetMoveDirection();
            _movementMap.Remove(_endCoordinate.ToString());
        }

        public Coordinate GetStartCoordinate()
        {
            return _startCoordinate;
        }

        public Coordinate GetEndCoordinate()
        {
            return _endCoordinate;
        }

        public Movement GetNextMovement(Coordinate currentCoordinate)
        {
            if (!_movementMap.ContainsKey(currentCoordinate.ToString()))
                throw new ArgumentOutOfRangeException();
            return _movementMap[currentCoordinate.ToString()];
        }

        public Coordinate GetNextCoordinate(Coordinate currentCoordinate) {
            if (!_movementMap.ContainsKey(currentCoordinate.ToString()))
                throw new ArgumentOutOfRangeException();
            return _movementMap[currentCoordinate.ToString()].GetMoveDirection() + currentCoordinate;
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            MovementPath movementPath = (MovementPath)obj;
            return (_movements.SequenceEqual(movementPath._movements) &&
                (_startCoordinate == movementPath._startCoordinate) &&
                (_endCoordinate == movementPath._endCoordinate));
        }

        public override string ToString()
        {
            string resultString = "";
            foreach (Movement movement in _movements)
            {
                resultString += movement.GetMoveDirection().ToString() + ":" + movement.GetMoveCost();
            }
            return resultString;
        }

        public override int GetHashCode()
        {
            return _startCoordinate.GetHashCode();
        }


    }
}