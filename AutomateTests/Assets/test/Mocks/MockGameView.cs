using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.MapModelComponents;

namespace AutomateTests.Mocks
{
    public class MockGameView : IGameView
    {
        private readonly List<MasterAction> _list;

        public MockGameView()
        {
            _list =new List<MasterAction>();
        }



        public List<MasterAction> GetHandledActions()
        {
            return _list;
        }




        public event ViewUpdate OnUpdateStart;
        public event ViewUpdate OnUpdate;
        public event ViewUpdate OnUpdateFinish;
        public void PerformCompleteUpdate()
        {
            PerformOnUpdateStart();
            PerformOnUpdate();
            PerformOnUpdateFinish();
        }

        public void PerformOnUpdate()
        {
            if (OnUpdate != null) OnUpdate.Invoke(new ViewUpdateArgs());
        }

        public void PerformOnUpdateStart()
        {
            if (OnUpdateStart != null) OnUpdateStart.Invoke(new ViewUpdateArgs());
        }

        public void PerformOnUpdateFinish()
        {
            if (OnUpdateFinish != null) OnUpdateFinish.Invoke(new ViewUpdateArgs());
        }

        public void PerformOnStart(Coordinate gameWorldSize)
        {
            throw new NotImplementedException();
        }

        public void PerformOnStart()
        {
            if (OnStart != null) OnStart.Invoke(new ViewUpdateArgs());
        }

        public event ViewUpdate OnStart;
        public IList<MasterAction> PullFromController()
        {
            var items = new List<MasterAction>();
            while (Controller.OutputSched.HasItems)
            {
                items.Add(Controller.OutputSched.Pull());
            }
            return items;
        }

        public IGameController Controller { get; set; }
        public void HandleAction(MasterAction action)
        {
            throw new NotImplementedException();
        }

        public event ViewHandleAction OnActionReady;
        public void PerformOnActionReady(ViewHandleActionArgs viewHandleArgs)
        {
            if (OnActionReady != null) OnActionReady.Invoke(viewHandleArgs);

        }

        IEnumerable<MasterAction> IGameView.PullFromController()
        {
            throw new NotImplementedException();
        }
    }

    public class TestingThreadWrapper : IHandlerResult<MasterAction>
    {
        public ThreadInfo PerformingThread { get; private set; }
        private readonly IHandlerResult<MasterAction> _handlerResult;

        public TestingThreadWrapper(ThreadInfo currentThread, IHandlerResult<MasterAction> handlerResult)
        {
            PerformingThread = currentThread;
            _handlerResult = handlerResult;

        }

        public IList<MasterAction> GetItems()
        {
            return _handlerResult.GetItems();
        }

        public bool IsInternal { get; set; }
    }
}