using System.Collections.Generic;
using Assets.src.Model.GameWorldComponents;
using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;

namespace Assets.src.Model
{
    public interface IModelAbstractionLayer
    {
        long CreateMovable(long WorldId, Coordinate spawnCoordinate, MovableType movableType);
        long CreateStructure(long WorldId, Coordinate spawnTopLeftCoordinate, StructureType structureType);
        //this is for movables
        Coordinate GetMovableCurrentCoordinate(long WorldId, long moveableId);
        //this is for structures
        Boundary GetStructureBoundary(long WorldId, long structureId);

        //Get ids of structures and movables
        List<long> GetMovableListInBoundary(long WorldId, Boundary selectionArea);
        List<long> GetMovableListInCoordinate(long WorldId, Coordinate selectionCoordinate);
        List<long> GetStructureListInBoundary(long WorldId, Boundary selectionArea);
        long GetStructureAtCoordinate(long WorldId, Coordinate selectionCoordinate);
        
        //motion issue movement related
        void IssueMoveCommand(long WorldId, long movableId, Coordinate targetCoordinate);
        bool IsMovableInMotion(long WorldId, long movableId);
        bool MoveMovableToNext(long WorldId, long moveableId);
        //motion information related
        Movement GetMovableNextMovement(long WorldId, long movableId);
        Coordinate GetMovableNextCoordinate(long WorldId, long movableId);

        //movable speed related
        double GetMovableSpeed(long WorldId, long moveableId);
        void SetMovableSpeed(long WorldId, long moveableId, double speed);
    }
}