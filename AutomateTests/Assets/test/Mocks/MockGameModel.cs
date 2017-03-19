using System;
using Assets.src.Controller;
using Assets.src.Controller.Interfaces;
using IObserverArgs = Assets.src.Controller.IObserverArgs;

namespace AutomateTests.test.Controller
{
    public class MockGameModel : IGameModel
    {
        private IPrimaryObserver _controllerPrimaryObserverAtModel;

        public MockGameModel(IPrimaryObserver controllerPrimaryObserverAtModel) {
            _controllerPrimaryObserverAtModel = controllerPrimaryObserverAtModel;
        }

        public IPrimaryObserver GetModelObservable()
        {
            return _controllerPrimaryObserverAtModel;
        }
    }
}