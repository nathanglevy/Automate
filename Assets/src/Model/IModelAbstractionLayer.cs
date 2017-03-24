using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;

namespace Assets.src.Model
{
    public interface IModelAbstractionLayer
    {
        MovementPath GetMovementPath();
    }
}