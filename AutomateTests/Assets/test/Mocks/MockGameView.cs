using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;

namespace AutomateTests.Mocks
{
    public class MockGameView : IGameView
    {
        private readonly List<MasterAction> _list;
        private event ViewCallBack callmeBack;
        public ConcurrentQueue<IHandlerResult> Results { get; }

        public MockGameView()
        {
            _list =new List<MasterAction>();
            callmeBack += HandleResults;
            Results = new ConcurrentQueue<IHandlerResult>();
        }

        private void HandleResults(IHandlerResult handlerResult)
        {
            Results.Enqueue(new TestingThreadWrapper(new ThreadInfo(null,Thread.CurrentThread), handlerResult));
        }


        public ViewCallBack GetCallBack()
        {
            return callmeBack;
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