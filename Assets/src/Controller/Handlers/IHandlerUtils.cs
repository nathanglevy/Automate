using Assets.src.Controller.Interfaces;
using src.Model;

namespace Assets.src.Controller.Handlers
{
    public interface IHandlerUtils    
    {
        IModelAbstractionLayer Model { get;  }

        ViewCallBack Enqueue { get; }
    }
}