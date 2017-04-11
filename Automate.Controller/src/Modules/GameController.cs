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
using Automate.Model;
using Model;

namespace Automate.Controller.Modules
{
    public class GameController : IGameController
    {
        private readonly List<IHandler<ObserverArgs>> _handlers;
        private event AcknowledgeActivation AcknowledgeActivation;
        private event HandleActivation HandlerActivation;
        private readonly ITimerScheduler<Abstracts.MasterAction> _timerSched;

        public GameController(IGameView view, IModelAbstractionLayer model)
        {
            Model = model;
            View = view;
            _handlers = new List<IHandler<ObserverArgs>>();

            // create the TimedOut Q and Link it
            _timerSched = new TimersSchedular<MasterAction>(new TimedOut<MasterAction>(Acknowledge));

            // create the controller-->View Schued
            OutputSched = new Scheduler<MasterAction>();
            HandlerActivation += Handle;
            AcknowledgeActivation += Acknowledge;

            // Link the 2 Sched
            OutputSched.OnPull += ForwardItemToTimerSched;
            
            // Link the View Update to the TimerSched
            view.onUpdate += ForwardUpdateToTimerSched;

            // register handlers
            _handlers.Add(new ViewSelectionHandler());
            _handlers.Add(new RightClickNotificationHandler());
            _handlers.Add(new AcknowledgeNotificationHandler());
        }

        private void ForwardUpdateToTimerSched(ViewUpdateArgs args)
        {
//            Console.Out.WriteLine("ForwardUpdate Fired");
            _timerSched.Update(new TimerSchudulerUpdateArgs() {Time = DateTime.Now});
        }

        private void ForwardItemToTimerSched(MasterAction item)
        {
//            Console.Out.WriteLine("ForwardItem Fired");
            //            _timerSched.Enqueue(DateTime.Now.Add(item.Duration),item);
            _timerSched.Enqueue(DateTime.Now.Add(new TimeSpan(0,0,0,0,100)),item);
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

                    Console.Out.WriteLine(String.Format("Handler {0} Fired with Args: {1}",handler.GetType(),args.GetType()));

                    //# create new thread to perform the action
                    AutoResetEvent syncEvent = new AutoResetEvent(false);
                    var subHandler = new Thread(delegate()
                    {
                        // Handle and Get Result
                        var handlerResult = handler.Handle(args,
                            new HandlerUtils(Model, HandlerActivation, AcknowledgeActivation));

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
                        IAcknowledgeResult<MasterAction> acknowledgeResult = handler.Acknowledge(action, new HandlerUtils(Model, HandlerActivation, AcknowledgeActivation));

                        // Push to Sched
                        OutputSched.GetPushInvoker().Invoke(acknowledgeResult);

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

        public IScheduler<MasterAction> OutputSched { get; private set; }
    }
}