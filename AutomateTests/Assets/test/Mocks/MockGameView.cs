using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;

namespace AutomateTests.test.Mocks
{
    public class MockGameView : IGameView
    {
        private readonly List<MasterAction> _list;
        public ConcurrentQueue<IHandlerResult> Results { get; }

        public MockGameView()
        {
            _list =new List<MasterAction>();
            Results = new ConcurrentQueue<IHandlerResult>();
        }

        private void HandleResults(IHandlerResult handlerResult)
        {
            Results.Enqueue(new TestingThreadWrapper(new ThreadInfo(null,Thread.CurrentThread), handlerResult));
        }



        public List<MasterAction> GetHandledActions()
        {
            return _list;
        }
    }

    public class TestingThreadWrapper : IHandlerResult
    {
        public ThreadInfo PerformingThread { get; }
        private readonly IHandlerResult _handlerResult;

        public TestingThreadWrapper(ThreadInfo currentThread, IHandlerResult handlerResult)
        {
            PerformingThread = currentThread;
            _handlerResult = handlerResult;

        }

        public IList<MasterAction> GetActions()
        {
            return _handlerResult.GetActions();
        }
    }
}