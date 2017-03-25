using System;
using System.Collections.Generic;
using src.Model.GameWorldComponents;
using src.Model.MapModelComponents;
using src.Model.PathFinding;

namespace src.Model
{
    public interface IModelAbstractionLayer
    {
        void FocusWorld(Guid worldId);
        Guid CreateGameWorld(Coordinate mapDimensions);
        Guid CreateMovable(Coordinate spawnCoordinate, MovableType movableType);
        Guid CreateStructure(Coordinate spawnTopLeftCoordinate, StructureType structureType);
        //this is for movables
        Coordinate GetMovableCurrentCoordinate(Guid moveableId);
        //this is for structures
        Boundary GetStructureBoundary(Guid structureId);

        //Get ids of structures and movables
        List<Guid> GetMovableListInBoundary(Boundary selectionArea);
        List<Guid> GetMovableListInCoordinate(Coordinate selectionCoordinate);
        List<Guid> GetStructureListInBoundary(Boundary selectionArea);
        Guid GetStructureAtCoordinate(Coordinate selectionCoordinate);
        
        //motion issue movement related
        void IssueMoveCommand(Guid movableId, Coordinate targetCoordinate);
        bool IsMovableInMotion(Guid movableId);
        Movement MoveMovableToNext(Guid moveableId);
        //motion information related
        Movement GetMovableNextMovement(Guid movableId);
        Coordinate GetMovableNextCoordinate(Guid movableId);

        //movable speed related
        double GetMovableSpeed(Guid moveableId);
        void SetMovableSpeed(Guid moveableId, double speed);
    }
}