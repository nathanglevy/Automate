namespace Automate.Controller.Interfaces
{
    public interface IGameView
    {
        event ViewUpdate onUpdate;
        void PerformUpdate();
    }

    public delegate void ViewUpdate(ViewUpdateArgs args);

    public class ViewUpdateArgs
    {
    }

}