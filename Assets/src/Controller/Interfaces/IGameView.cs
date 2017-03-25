namespace Assets.src.Controller.Interfaces
{
    public interface IGameView
    {
        ViewCallBack GetCallBack();
    }

    public delegate void ViewCallBack(IHandlerResult actions);
}