﻿using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.GoAndDoSomething;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Handlers.PlaceAnObject;
using Automate.Controller.Handlers.RequirementsHandler;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Handlers.TaskActionHandler;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Controller.Interfaces;
using Automate.Model;
using Automate.Model.CellComponents;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.StructureComponents;

namespace Automate.Controller.Modules
{
    public class GameController : IGameController
    {
        private readonly List<IHandler<IObserverArgs>> _handlers;
        private event AcknowledgeActivation AcknowledgeActivation;
        private event HandleActivation HandlerActivation;
        private readonly ITimerScheduler<Abstracts.MasterAction> _timerSched;

        // events
        public event ControllerNotification OnPreHandle;
        public event ControllerNotification OnPostHandle;
        public event ControllerNotification OnFinishHandle;


        public bool MultiThreaded { get; set; }

        public GameController(IGameView view)
        {
            GameWorldGuid = Guid.Empty;
            View = view;
            // register current Controller
            view.Controller = this;

            MultiThreaded = true;

            _handlers = new List<IHandler<IObserverArgs>>();

            // create the TimedOut Q and Link it
            _timerSched = new TimersScheduler<MasterAction>(AssignAsOverAndHandle);

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
            //AcknowledgeActivation += Acknowledge;

            // Link the 2 Sched
            OutputSched.OnPull += ForwardItemToTimerSched;

            // register handlers
            _handlers.Add(new RequirementsHandler());

            _handlers.Add(new GoAndPickUpTaskHandler());
            _handlers.Add(new GoAndDeliverTaskHandler())
                ;
            _handlers.Add(new ViewSelectionHandler());
            _handlers.Add(new RightClickNotificationHandler());
            _handlers.Add(new PlaceAnObjectRequestHandler());

            _handlers.Add(new MoveActionHandler());
            _handlers.Add(new StartMoveActionHandler());

            _handlers.Add(new PickUpActionHandler());
            _handlers.Add(new DeliverActionHandler());

            _handlers.Add(new TaskHandler());
            _handlers.Add(new TaskActionHandler());



        }

        private IList<ThreadInfo> AssignAsOverAndHandle(MasterAction args)
        {
            // Listner for Timeout, adding Flag to mark action is over
            args.IsActionHasOver = true;

            // Call Handle to Handle The Action (UpdateModel/GetNext...)
            return Handle(args);
        }

        private void InitGameWorld(ViewUpdateArgs args)
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(args.GameWorldSize);
            FocusGameWorld(gameWorldItem.Guid);

        }


        private void OnViewUpdate(ViewUpdateArgs args)
        {
            // Push any pending items at model to view
            PushFromModelToView(GameUniverse.GetGameWorldItemById(GameWorldGuid), OutputSched);

            // forward the update to the Timer Sched
            ForwardUpdateToTimerSched(args);

            // Handle All PickUp/Delivery/Build/... Requirments
            HandleRequirements(args);


        }

        private void HandleRequirements(ViewUpdateArgs args)
        {
            // Foreach Sorted Requirment
                // Foreach Idle Movable
                    // IfCanMovableDo(Requirment)

                    // Calc Movable Job ScenarioCost (return Min)(Dest,PickUPs,Stoargs,movables)
                        // Calculate Movable with component directly to Dest ScenarioCost
                        // Movable to Movable Transfer directly to Dest

                        // movable with component to Storage to Dest
                        // Movable with Component to Pickup to Dest

                        // Movable without anything to pickup to Dest
                        // Movable without anything to Storate to Dest

                    // Add As Candidate

            // At this point we have several candidates for the job
             // Pick the Cheapest movable and Execute the Requriment


            var gameWorld = GameUniverse.GetGameWorldItemById(GameWorldGuid);
            var requirementsPackage = new RequirementsPackage(gameWorld.RequirementAgent);

            // Call Handle Method
            Handle(requirementsPackage);
        }

        private void PushFromModelToView(IGameWorld gameWorldItem,
            IScheduler<MasterAction> scheduler)
        {

            if (gameWorldItem.IsThereAnItemToBePlaced())
            {
                foreach (var item in gameWorldItem.GetItemsToBePlaced())
                {
                    scheduler.Enqueue(new PlaceAGameObjectAction(item.ItemType, item.Guid, item,
                        GetCoordinate(item)));
                }

                gameWorldItem.ClearItemsToBePlaced();
            }

        }

        //TODO: MOVE TO Item Object
        private Coordinate GetCoordinate(Item item)
        {
            switch (item.ItemType)
            {
                case ItemType.Cell:
                    return (item as CellItem)?.Coordinate;
                case ItemType.Movable:
                    return (item as IMovable)?.CurrentCoordinate;
                case ItemType.Structure:
                    return (item as IStructure)?.Boundary.topLeft;
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


        public Guid GameWorldGuid { get; private set; }
        public IGameView View { get; private set; }

        public IList<ThreadInfo> Handle(IObserverArgs args)
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
                    if (MultiThreaded)
                    {
                        var subHandler = new Thread(HandlePushAndNotify(args, handler, syncEvent))
                        { Name = String.Format("{0}_HandleWorkerThread", handler.GetType().ToString()) };
                        threads.Add(new ThreadInfo(syncEvent, subHandler));
                        subHandler.Start();
                    }
                    else
                    {
                        HandlePushAndNotify(args, handler, syncEvent).Invoke();
                    }
                }
            }
            return threads;
        }

        

        private ThreadStart HandlePushAndNotify(IObserverArgs args, IHandler<IObserverArgs> handler, AutoResetEvent syncEvent)
        {
            return delegate ()
            {

                // invoke Pre Handle
                var handlerUtils = new HandlerUtils(GameWorldGuid, HandlerActivation, AcknowledgeActivation);
                OnPreHandle?.Invoke(new ControllerNotificationArgs(args, handlerUtils));

                // Handle and Get Result
                
                var handlerResult = handler.Handle(args,handlerUtils);

                // invoke Pre Handle
                OnPostHandle?.Invoke(new ControllerNotificationArgs(args, handlerUtils));

                // Push to Sched
                if (handlerResult.IsInternal)
                {
                    foreach (var item in handlerResult.GetItems())
                    {
                        var waitSync = handlerUtils.InvokeHandler(item);
                        foreach (var threadInfo in waitSync)
                        {
                            threadInfo.SyncEvent.WaitOne();
                        }
                        //HandlePushAndNotify(item, handler, new AutoResetEvent(false)).Invoke();
                    }
                }
                else
                {
                    OutputSched.GetPushInvoker().Invoke(handlerResult);
                }
                

                // invoke on Finish
                OnFinishHandle?.Invoke(new ControllerNotificationArgs(args, handlerUtils) {Utils = handlerUtils });

                // resume any waiting threads
                syncEvent.Set();


            };
        }

        //protected IList<ThreadInfo> Acknowledge(MasterAction action)
        //{
        //    IList<ThreadInfo> threads = new List<ThreadInfo>();
        //    foreach (var handler in _handlers)
        //    {
        //        if (handler.CanAcknowledge(action))
        //        {

        //            //# create new thread to perform the action
        //            AutoResetEvent syncEvent = new AutoResetEvent(false);
        //            if (MultiThreaded)
        //            {
        //                var subHandler = new Thread(AcknowledgePushAndNotify(action, handler, syncEvent))
        //                { Name = String.Format("{0}_AcknowledgeWorkerThread", handler.GetType().ToString()) };
        //                threads.Add(new ThreadInfo(syncEvent, subHandler));
        //                subHandler.Start();
        //            }
        //            else
        //            {
        //                AcknowledgePushAndNotify(action, handler, syncEvent).Invoke();
        //            }
        //        }
        //    }
        //    return threads;
        //}

        //private ThreadStart AcknowledgePushAndNotify(MasterAction action, IHandler<ObserverArgs> handler, AutoResetEvent syncEvent)
        //{
        //    return delegate ()
        //    {
        //        // Handle and Get Result
        //        IAcknowledgeResult<MasterAction> acknowledgeResult = handler.Acknowledge(action,
        //            new HandlerUtils(GameWorldGuid, HandlerActivation, AcknowledgeActivation));

        //        // Push to Sched
        //        OutputSched.GetPushInvoker().Invoke(acknowledgeResult);

        //        // resume any waiting threads
        //        syncEvent.Set();
        //    };
        //}

        public int GetHandlersCount()
        {
            return _handlers.Count;
        }

        public void RegisterHandler(IHandler<IObserverArgs> handler)
        {
            _handlers.Add(handler);
        }


        public IScheduler<MasterAction> OutputSched { get; private set; }

     
        public bool HasFocusedGameWorld
        {
            get { return !GameWorldGuid.Equals(Guid.Empty); }
        }

        public void FocusGameWorld(Guid gameWorldId)
        {
            GameWorldGuid = gameWorldId;
        }

        public Guid UnfocusGameWorld()
        {
            Guid focusedWorld = GameWorldGuid;
            GameWorldGuid = Guid.Empty;
            return focusedWorld;
        }

        public void OnUpdateFinish(ViewUpdateArgs args)
        {
            //throw new NotImplementedException();
        }

        public void OnUpdate(ViewUpdateArgs args)
        {
            //throw new NotImplementedException();
        }

        public void OnUpdateStart(ViewUpdateArgs args)
        {
            //throw new NotImplementedException();
        }

    }

    internal class ThreadControl
    {
        public bool CanRun { get; set; }
    }
}