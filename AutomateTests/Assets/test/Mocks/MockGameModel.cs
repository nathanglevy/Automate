using System;
using System.Collections.Generic;
using Assets.src.Controller.Interfaces;
using Assets.src.Model;
using Assets.src.Model.GameWorldComponents;
using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;

namespace AutomateTests.Mocks
{
    public class MockGameModel : IModelAbstractionLayer
    {
        public MovementPath GetMovementPath()
        {
            return new MovementPath(new Coordinate(10,10,0));
        }

        public List<String> GetPlayersInSelection(Coordinate notificationUpperLeft, Coordinate notificationBottomRight)
        {
            List<string> ids = new List<string>();
            ids.Add("AhmadHamdan");
            ids.Add("NaphLevy");
            return ids;
        }


        public Coordinate GetPlayerCoordinate(string guid)
        {
            return new Coordinate(10, 10, 0);
        }

        public void FocusWorld(Guid WorldId)
        {
            throw new NotImplementedException();
        }

        public Guid CreateGameWorld(Coordinate mapDimensions)
        {
            throw new NotImplementedException();
        }

        public Guid CreateMovable(Coordinate spawnCoordinate, MovableType movableType)
        {
            throw new NotImplementedException();
        }

        public Guid CreateStructure(Coordinate spawnTopLeftCoordinate, StructureType structureType)
        {
            throw new NotImplementedException();
        }

        public Coordinate GetMovableCurrentCoordinate(Guid moveableId)
        {
            return new Coordinate(10, 10, 0);
        }

        public Boundary GetStructureBoundary(Guid structureId)
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetMovableListInBoundary(Boundary selectionArea)
        {
            
            return new List<Guid>() {Guid.NewGuid(),Guid.NewGuid()};

        }
        public List<Guid> GetMovableListInCoordinate(Coordinate selectionCoordinate)
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetStructureListInBoundary(Boundary selectionArea)
        {
            throw new NotImplementedException();
        }

        public Guid GetStructureAtCoordinate(Coordinate selectionCoordinate)
        {
            throw new NotImplementedException();
        }

        public void IssueMoveCommand(Guid movableId, Coordinate targetCoordinate)
        {
            throw new NotImplementedException();
        }

        public bool IsMovableInMotion(Guid movableId)
        {
            throw new NotImplementedException();
        }

        public Movement MoveMovableToNext(Guid moveableId)
        {
            throw new NotImplementedException();
        }

        public Movement GetMovableNextMovement(Guid movableId)
        {
            throw new NotImplementedException();
        }

        public Coordinate GetMovableNextCoordinate(Guid movableId)
        {
            throw new NotImplementedException();
        }

        public double GetMovableSpeed(Guid moveableId)
        {
            throw new NotImplementedException();
        }

        public void SetMovableSpeed(Guid moveableId, double speed)
        {
            throw new NotImplementedException();
        }
    }
}