using System.Collections.Generic;
using Assets.src.Model.MapModelComponents;

namespace Assets.src.Controller.Interfaces
{
    public interface IGameModel
    {
        IPrimaryObserver GetModelPrimaryObserver();


        IEnumerable<CellInfo> GetCellsInRange(Coordinate selectionArgsUpperLeft, Coordinate selectionArgsLowerRight);

        void revertPassableState(CellInfo cellInfo);
    }
}