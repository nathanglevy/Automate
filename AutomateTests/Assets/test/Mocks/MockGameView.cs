using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;

namespace AutomateTests.Mocks
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
            if (onUpdate != null)
                onUpdate(new ViewUpdateArgs());
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