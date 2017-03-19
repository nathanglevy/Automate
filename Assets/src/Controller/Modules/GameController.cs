using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller.Modules
{
    public class GameController : IGameController
    {
        public IPrimaryObserver ModelObservable { get; private set; }
        public IPrimaryObserver ViewControllerObserver { get; private set; }
        public IPrimaryObserver ModelControllerObserver { get; private set; }
        public IPrimaryObserver ViewObservable { get; private set; }

        public GameController(IPrimaryObserver viewObservable,
            IPrimaryObserver modelObservable,
            IPrimaryObserver viewControllerObserver,
            IPrimaryObserver modelControllerObserver)
        {
            ViewObservable = viewObservable;
            ModelObservable = modelObservable;
            ViewControllerObserver = viewControllerObserver;
            ModelControllerObserver = modelControllerObserver;

        }
    }
}