using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;

namespace Assets.src.Model
{
    public interface IModelAbstractionLayer
    {
        long CreateMovable(long WorldId, Coordinate spawnCoordinate);
        void IssueMoveCommand(long WorldId, long movableId, Coordinate targetCoordinate);
        bool IsMovableInMotion(long WorldId, long movableId);
        MovementPackage GetNextMovement(long WorldId);
        MovementPackage MoveMovableToNext(long WorldId, long moveableId);
        double GetMovableSpeed(long WorldId, long moveableId);
        Coordinate GetMovableCurrentCoordinate(long WorldId, long moveableId);
    }

    public struct MovementPackage
    {
        Coordinate targetCoordinate;
        Movement movement;
    }
}