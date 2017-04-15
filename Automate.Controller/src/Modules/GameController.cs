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
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Modules
{
    public class GameController : IGameController
    {
        private readonly List<IHandler<ObserverArgs>> _handlers;
        private event AcknowledgeActivation AcknowledgeActivation;
        private event HandleActivation HandlerActivation;
        private readonly ITimerScheduler<Abstracts.MasterAction> _timerSched;

        public GameController(IGameView view)
        {
            Model = Guid.Empty;
            View = view;

            // register current Controller
            view.Controller = this;

            _handlers = new List<IHandler<ObserverArgs>>();

            // create the TimedOut Q and Link it
            _timerSched = new TimersSchedular<MasterAction>(new TimedOut<MasterAction>(Acknowledge));

            // create the controller-->View Schued
            OutputSched = new Scheduler<MasterAction>();

            // Register OnStart
            view.OnStart += InitGameWorld;

            // Link Update Start and Finish to Sched
            view.OnUpdateStart += OutputSched.OnPullStart;
            view.OnUpdateFinish += OutputSched.OnPullFinish;

            // Link the View Update to the TimerSched
            view.OnUpdate += OnViewUpdate;
            HandlerActivation += Handle;
            AcknowledgeActivation += Acknowledge;

            // Link the 2 Sched
            OutputSched.OnPull += ForwardItemToTimerSched;

            // register handlers
            _handlers.Add(new ViewSelectionHandler());
            _handlers.Add(new RightClickNotificationHandler());
            _handlers.Add(new AcknowledgeNotificationHandler());

        }

        private void InitGameWorld(ViewUpdateArgs args)
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
            gameWorldItem.CreateMovable(new Coordinate(10, 1, 0), MovableType.NormalHuman);
            gameWorldItem.CreateMovable(new Coordinate(1, 3, 0), MovableType.NormalHuman);
            gameWorldItem.CreateStructure(new Coordinate(5, 4, 1), new Coordinate(1, 1, 1), StructureType.Basic);
            FocusGameWorld(gameWorldItem.Guid);

        }


        private void OnViewUpdate(ViewUpdateArgs args)
        {
            // Push any pending items at model to view
            PushFromModelToView(GameUniverse.GetGameWorldItemById(Model), OutputSched);

            // forward the update to the Timer Sched
            ForwardUpdateToTimerSched(args);
        }

        private void PushFromModelToView(GameWorldItem gameWorldItem,
            IScheduler<MasterAction> scheduler)
        {

            if (gameWorldItem.IsThereAnItemToBePlaced())
            {
                foreach (var item in gameWorldItem.GetItemsToBePlaced())
                {
                    scheduler.Enqueue(new PlaceAGameObjectAction(item.Type, item.Guid, item,
                        GetCoordinate(item)));
                }
                gameWorldItem.ClearItemsToBePlaced();
            }

        }

        //TODO: MOVE TO Item Object
        private Coordinate GetCoordinate(Item item)
        {
            switch (item.Type)
            {
                case ItemType.Cell:
                    return (item as CellItem).Coordinate;
                case ItemType.Movable:
                    return (item as MovableItem).CurrentCoordiate;
                case ItemType.Structure:
                    return (item as StructureItem).StructureBoundary.topLeft;
                default:
                    throw new Exception("Unexpected item type!");
            }
        }

        private void ForwardUpdateToTimerSched(ViewUpdateArgs args)
        {
            //            Console.Out.WriteLine("ForwardUpdate Fired");
            _timerSched.Update(new TimerSchudulerUpdateArgs() { Time = DateTime.Now });
        }

        private void ForwardItemToTimerSched(MasterAction item)
        {
            //            Console.Out.WriteLine("ForwardItem Fired");
            //            _timerSched.Enqueue(DateTime.Now.Add(item.Duration),item);
            if (item.NeedAcknowledge)
                _timerSched.Enqueue(DateTime.Now.Add(item.Duration), item);
        }


        public Guid Model { get; private set; }
        public IGameView View { get; private set; }

        public IList<ThreadInfo> Handle(ObserverArgs args)
        {
            IList<ThreadInfo> threads = new List<ThreadInfo>();
            foreach (var handler in _handlers)
            {
                if (handler.CanHandle(args))
                {

                    Console.Out.WriteLine(String.Format("Handler {0} Fired with Args: {1}", handler.GetType(),
                        args.GetType()));

                    //# create new thread to perform the action
                    AutoResetEvent syncEvent = new AutoResetEvent(false);
                    var subHandler = new Thread(delegate ()
                    {
                        // Handle and Get Result
                        var handlerResult = handler.Handle(args,
                            new HandlerUtils(Model, HandlerActivation, AcknowledgeActivation));

                        // Push to Sched
                        OutputSched.GetPushInvoker().Invoke(handlerResult);

                        // resume any waiting threads
                        syncEvent.Set();
                    })
                    { Name = String.Format("{0}_HandleWorkerThread", handler.GetType().ToString()) };
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
                            IAcknowledgeResult<MasterAction> acknowledgeResult = handler.Acknowledge(action,
                                new HandlerUtils(Model, HandlerActivation, AcknowledgeActivation));

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

        public void OnUpdateFinish(ViewUpdateArgs args)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(ViewUpdateArgs args)
        {
            throw new NotImplementedException();
        }

        public void OnUpdateStart(ViewUpdateArgs args)
        {
            throw new NotImplementedException();
        }

        public bool HasFocusedGameWorld
        {
            get { return !Model.Equals(Guid.Empty); }
        }

        public void FocusGameWorld(Guid gameWorldId)
        {
            Model = gameWorldId;
        }

        public Guid UnfocusGameWorld()
        {
            Guid focusedWorld = Model;
            Model = Guid.Empty;
            return focusedWorld;
        }
    }

    internal class ThreadControl
    {
        public bool CanRun { get; set; }
    }
}