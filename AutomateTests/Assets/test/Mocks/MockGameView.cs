using Assets.src.Controller.Interfaces;

namespace AutomateTests.Mocks
{
    public class MockGameView : IGameView
    {
        private readonly IPrimaryObserver _viewControllerObserver;

        public MockGameView(IPrimaryObserver viewControllerObserver)
        {
            _viewControllerObserver = viewControllerObserver;
        }

        public IPrimaryObserver GetViewPrimaryObserver()
        {
            return _viewControllerObserver;
        }

        public IPrimaryObserver GetViewObservable()
        {
            throw new System.NotImplementedException();
        }
    }
}