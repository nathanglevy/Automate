using System;
using System.Collections.Generic;
using System.Threading;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using AutomateTests.test.Controller;
using src.Model;

namespace Assets.src.Controller.Modules
{
    public class GameController : IGameController
    {
        private readonly List<IHandler<ObserverArgs>> _handlers;


        public GameController(IGameView view, IModelAbstractionLayer model)
        {
            Model = model;
            View = view;
            _handlers = new List<IHandler<ObserverArgs>>();
        }


        public IModelAbstractionLayer Model { get; private set; }
        public IGameView View { get; private set; }

        public IList<ThreadInfo> Handle(ObserverArgs args, ViewCallBack viewCallBack)
        {
            IList<ThreadInfo> threads = new List<ThreadInfo>();
            foreach (var handler in _handlers)
            {
                if (handler.CanHandle(args))
                {
                    //# create new thread to perform the action
                    AutoResetEvent syncEvent = new AutoResetEvent(false);
                    var subHandler = new Thread(delegate()
                    {
                        // Handle and Get Result
                        var handlerResult = handler.Handle(args, new HandlerUtils(Model, viewCallBack));

                        // Push to WhoEver Waiting the results
                        viewCallBack(handlerResult);

                        // resume any waiting threads
                        syncEvent.Set();
                    }) {Name = String.Format("{0}_WorkerThread", handler.GetType().ToString())};
                    threads.Add(new ThreadInfo(syncEvent, subHandler));
                    subHandler.Start();
                }
            }
            return threads;
        }

        public int GetHandlersCount()
        {
            return _handlers.Count;
        }

        public void RegisterHandler(IHandler<ObserverArgs> handler)
        {
            _handlers.Add(handler);
        }
    }
}