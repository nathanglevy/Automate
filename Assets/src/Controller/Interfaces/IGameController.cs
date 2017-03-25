using System.Collections.Generic;
using System.Threading;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Modules;
using Assets.src.Model;

namespace Assets.src.Controller.Interfaces
{
    public interface IGameController
    {
        IModelAbstractionLayer Model { get; }
        IGameView View { get; }
        IList<ThreadInfo> Handle(ObserverArgs args, ViewCallBack viewCallBack);
        int GetHandlersCount();
        void RegisterHandler(IHandler<ObserverArgs> handler);
    }
}
