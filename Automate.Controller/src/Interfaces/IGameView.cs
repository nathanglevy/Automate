using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Modules;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Interfaces
{
    public interface IGameView
    {
        void PerformCompleteUpdate();
        void PerformOnUpdate();
        void PerformOnUpdateStart();
        void PerformOnUpdateFinish();
        void PerformOnStart(Coordinate gameWorldSize);

        void PerformOnActionReady(ViewHandleActionArgs viewHandleArgs);


        event ViewUpdate OnUpdateStart;
        event ViewUpdate OnUpdate;
        event ViewUpdate OnUpdateFinish;
        event ViewUpdate OnStart;

        event ViewHandleAction OnActionReady;


        IEnumerable<MasterAction> PullFromController();
        IGameController Controller { get; set; }
        void HandleAction(MasterAction action);
 
    }

    public delegate void ViewHandleAction(ViewHandleActionArgs args);

    public class ViewHandleActionArgs
    {
        public MasterAction Action { get; set; }
    }

    public delegate void ViewUpdate(ViewUpdateArgs args);

    public class ViewUpdateArgs
    {
        public List<ThreadInfo> ThreadsInfo { get; set; }
        public Coordinate GameWorldSize { get; set; }
    }

}