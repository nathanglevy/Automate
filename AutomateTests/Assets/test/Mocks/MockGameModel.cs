﻿using System;
using System.Collections.Generic;
using Assets.src.Controller.Interfaces;
using Assets.src.Model.MapModelComponents;

namespace AutomateTests.Mocks
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

        public IPrimaryObserver GetModelObservable()
        {
            throw new NotImplementedException();
        }
    }
}