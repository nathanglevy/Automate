using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;

namespace AutomateTests.test.Mocks
{
    public class MockGameView : IGameView
    {
        private readonly List<MasterAction> _list;
        public ConcurrentQueue<IHandlerResult<MasterAction>> Results { get; private set; }

        public MockGameView()
        {
            _list =new List<MasterAction>();
            Results = new ConcurrentQueue<IHandlerResult<MasterAction>>();
        }

        private void HandleResults(IHandlerResult<MasterAction> handlerResult)
        {
            Results.Enqueue(new TestingThreadWrapper(new ThreadInfo(null,Thread.CurrentThread), handlerResult));
        }



        public List<MasterAction> GetHandledActions()
        {
            return _list;
        }

        public event ViewUpdate onUpdate;


        public void PerformUpdate()
        {
            if (onUpdate != null) onUpdate?.Invoke(new ViewUpdateArgs());
        }

        public event ViewUpdate OnUpdateStart;
        public event ViewUpdate OnUpdate;
        public event ViewUpdate OnUpdateFinish;
        public void PerformCompleteUpdate()
        {
            throw new NotImplementedException();
        }

        public void PerformOnUpdate()
        {
            throw new NotImplementedException();
        }

        public void PerformOnUpdateStart()
        {
            throw new NotImplementedException();
        }

        public void PerformOnUpdateFinish()
        {
            throw new NotImplementedException();
        }

        public void PerformOnStart()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}