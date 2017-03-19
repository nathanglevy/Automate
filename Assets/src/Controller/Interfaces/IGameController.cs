using Assets.src.Controller.Abstracts;

namespace Assets.src.Controller.Interfaces
{
    public interface IGameController
    {
        IPrimaryObserver ModelObservable { get; }
        IPrimaryObserver ViewControllerObserver { get; }
        IPrimaryObserver ModelControllerObserver { get; }
        IPrimaryObserver ViewObservable { get; }
    }
}
