using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller.Abstracts
{
    public abstract class ObserverArgs : IObserverArgs
    {
        public string TargetId { get; private set; }
    }
}