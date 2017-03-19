using Assets.src.Controller;
using Assets.src.Controller.Interfaces;
using Automate.Assets.src.Controller;
using IObserverArgs = Assets.src.Controller.IObserverArgs;

namespace AutomateTests.test.Controller
{
    public class MockGameView : IGameView
    {
        private readonly IPrimaryObserver _viewControllerObserver;

        public MockGameView(IPrimaryObserver viewControllerObserver)
        {
            _viewControllerObserver = viewControllerObserver;
        }

        public IPrimaryObserver GetViewObservable()
        {
            return _viewControllerObserver;
        }
    }
}