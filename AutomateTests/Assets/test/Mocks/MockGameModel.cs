using System;
using System.Collections.Generic;
using Assets.src.Controller;
using Assets.src.Controller.Interfaces;
using Assets.src.PathFinding.MapModelComponents;
using IObserverArgs = Assets.src.Controller.IObserverArgs;

namespace AutomateTests.test.Controller
{
    public class MockGameModel : IGameModel
    {
        private IPrimaryObserver _controllerPrimaryObserverAtModel;

        public MockGameModel(IPrimaryObserver controllerPrimaryObserverAtModel) {
            _controllerPrimaryObserverAtModel = controllerPrimaryObserverAtModel;
        }

        public IPrimaryObserver GetModelPrimaryObserver()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CellInfo> GetCellsInRange(Coordinate selectionArgsUpperLeft, Coordinate selectionArgsLowerRight)
        {
            throw new NotImplementedException();
        }

        public void revertPassableState(CellInfo cellInfo)
        {
            throw new NotImplementedException();
        }
    }
}