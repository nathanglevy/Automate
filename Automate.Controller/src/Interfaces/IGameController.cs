using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model;

namespace Automate.Controller.Interfaces
{
    public interface IGameController
    {
        IModelAbstractionLayer Model { get; }
        IGameView View { get; }
        IScheduler OutputSched { get;}
        IList<ThreadInfo> Handle(ObserverArgs args);
        int GetHandlersCount();
        void RegisterHandler(IHandler<ObserverArgs> handler);
    }
}
