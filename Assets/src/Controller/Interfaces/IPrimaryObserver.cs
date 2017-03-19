namespace Assets.src.Controller.Interfaces
{
    public interface IPrimaryObserver
    {
        void RegisterObserver(ISecondryObserver secondryObserver);
        
        void Invoke<U>(U invokeArgs) where U : IObserverArgs;

        int GetObserversCount();
    }
}