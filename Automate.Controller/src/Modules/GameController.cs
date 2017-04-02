using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.AcknowledgeNotification;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Interfaces;
using Automate.Model.src;

namespace Automate.Controller.Modules
{
    public class GameController : IGameController
    {
        private readonly List<IHandler<ObserverArgs>> _handlers;
        private event AcknowledgeActivation _acknowledgeActivation;
        private event HandleActivation _handlerActivation;

        public GameController(IGameView view, IModelAbstractionLayer model)
        {
            Model = model;
            View = view;
            _handlers = new List<IHandler<ObserverArgs>>();
            OutputSched = new Scheduler();
            _handlerActivation += Handle;
            _acknowledgeActivation += Acknowledge;

            // register handlers
            _handlers.Add(new ViewSelectionHandler());
            _handlers.Add(new RightClickNotificationHandler());
            _handlers.Add(new AcknowledgeNotificationHandler());
        }


        public IModelAbstractionLayer Model { get; private set; }
        public IGameView View { get; private set; }

        public IList<ThreadInfo> Handle(ObserverArgs args)
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
                        var handlerResult = handler.Handle(args,
                            new HandlerUtils(Model, _handlerActivation, _acknowledgeActivation));

                        // Push to Sched
                        OutputSched.GetPushInvoker().Invoke(handlerResult);

                        // resume any waiting threads
                        syncEvent.Set();
                    }) {Name = String.Format("{0}_HandleWorkerThread", handler.GetType().ToString())};
                    threads.Add(new ThreadInfo(syncEvent, subHandler));
                    subHandler.Start();
                }
            }
            return threads;
        }

        protected IList<ThreadInfo> Acknowledge(MasterAction action)
        {
            IList<ThreadInfo> threads = new List<ThreadInfo>();
            foreach (var handler in _handlers)
            {
                if (handler.CanAcknowledge(action))
                {
                    //# create new thread to perform the action
                    AutoResetEvent syncEvent = new AutoResetEvent(false);
                    var subHandler = new Thread(delegate ()
                    {
                        // Handle and Get Result
                        var handlerResult = handler.Acknowledge(action, new HandlerUtils(Model, _handlerActivation, _acknowledgeActivation));

                        // Push to Sched
                        OutputSched.GetPushInvoker().Invoke(handlerResult);

                        // resume any waiting threads
                        syncEvent.Set();
                    })
                    { Name = String.Format("{0}_AcknowledgeWorkerThread", handler.GetType().ToString()) };
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

        public IScheduler OutputSched { get; private set; }
    }
}