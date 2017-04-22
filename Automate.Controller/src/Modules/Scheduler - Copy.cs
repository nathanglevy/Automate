using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;
using UnityEngine;
using Object = System.Object;
using ThreadPriority = System.Threading.ThreadPriority;

namespace Automate.Controller.Modules
{
    public class BackupScheduler<MasterAction> : IScheduler<MasterAction>
    {
        // collection
        //private ConcurrentQueue<MasterAction> _pushOnlyQueue = new ConcurrentQueue<MasterAction>();
        private Queue<MasterAction> _pushOnlyQueue = new Queue<MasterAction>();
        private Queue<MasterAction> _pullOnlyQueue = new Queue<MasterAction>();
        private AutoResetEvent _copyActionsAutoResetEvent = new AutoResetEvent(false);
        private Object _pushOnlyLock = new Object();
        private Object _pullOnlylock = new Object();
        private Thread _copyThread;
        Logger _logger = new Logger(new AutomateLogHandler());
        // events
        private event HandlerResultListner<MasterAction> _enqueueListener;
        public event NotifyOnPull<MasterAction> OnPull;
        public event NotifyOnPull<MasterAction> OnEnqueue;


        public BackupScheduler()
        {
            _enqueueListener += PushResultsToQueue;
            _logger.logEnabled = true;
        }

      
        private void PushResultsToQueue(IHandlerResult<MasterAction> handlerResult)
        {
            Enqueue(handlerResult.GetItems());
        }

        public int ItemsCount
        {
            get
            {
                return _pullOnlyQueue.Count;
            }
        }

        public void Enqueue(IList<MasterAction> items)
        {
            foreach (var action in items)
            {
                Enqueue(action);
            }

        }

        public bool HasItems
        {
            //get { return !_pushOnlyQueue.IsEmpty; }
//            get { return _pushOnlyQueue.Count != 0; }
            get { return _pullOnlyQueue.Count != 0; }
        }

        public MasterAction Pull()
        {
            _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                "Pull Method invoked");
            MasterAction deqMasterAction = default(MasterAction);
            lock (_pullOnlylock)
            {
                _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                    "Lock Acquired");
                if (HasItems)
                {
                    _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                        "Dequeue an action and send using OnPull Event");

                    deqMasterAction = _pullOnlyQueue.Dequeue();

                    _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                        "Action being Dequeued, " + deqMasterAction.ToString());

                }
                else
                {
                    throw new Exception("Queue is empty, cannot pull");
                }
            }
            Console.Out.WriteLine("Dequeued an item from Q, ID:" + deqMasterAction.ToString());

            // Fire OnPull To All Listners
//            deqMasterAction.N
            if (OnPull != null)
            {
                _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                    "invoke on Pull Event to all listeners " + deqMasterAction.ToString());
                OnPull(deqMasterAction);
            }


            // return to requestor
            return deqMasterAction;



            
        }

        public HandlerResultListner<MasterAction> GetPushInvoker()
        {
            return _enqueueListener;
        }

        public void Enqueue(MasterAction item)
        {
            lock (_pushOnlyLock)
            {
                _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                    "Adding an item to PushOnly, item: " + item.ToString());
                _pushOnlyQueue.Enqueue(item);
            }

        }

        public void OnPullFinish(ViewUpdateArgs args)
        {
            if (_copyThread == null || !_copyThread.IsAlive)
            {
                _copyThread = new Thread(CopyToPullOnlyQ);
                _copyThread.Priority = ThreadPriority.Highest;
                _copyThread.Start();
            }
            _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                "Creating/Resuming the thread which copy from PushOnlyQ to PullOnlyQ");
            _copyActionsAutoResetEvent.Set();
        }

        public void OnPullStart(ViewUpdateArgs args)
        {
            _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                "stopping/pausing the thread which copy from PushOnlyQ to PullOnlyQ");
           // CopyToPullOnlyQ();
            _copyActionsAutoResetEvent.Reset();

        }

        private void CopyToPullOnlyQ()
        {
            _logger.Log(LogType.Log, LogTag.Scheduler.ToString(), "Copy To Pull Only Invoked");
            var count = 0;
            var pushOnlyItemsCount = 0;
            lock (_pushOnlyLock)
            {
                pushOnlyItemsCount = _pushOnlyQueue.Count;
            }
            while (pushOnlyItemsCount > 0)
            {
                _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                    "Push Only Has Actions, check if we can start copy or we lock is enabled");
                _copyActionsAutoResetEvent.WaitOne();
                _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                    "_lock is disabled, will try to move from pushOnlyToPullOnly Q");
                MasterAction itemToBeCopied;
                lock (_pushOnlyLock)
                {
                    itemToBeCopied = _pushOnlyQueue.Dequeue();
                    pushOnlyItemsCount--;
                }
                _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                    "Dequeue an action from PushOnly, acquire lock and push to pull only. " +
                    itemToBeCopied.ToString());
                lock (_pullOnlyQueue)
                {
                    _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                        "lock aquired, add to PullOnly Q");
                    _pullOnlyQueue.Enqueue(itemToBeCopied);
                    count++;
                }
                if (OnEnqueue != null)
                {
                    _logger.Log(LogType.Log, LogTag.Scheduler.ToString(),
                        "invoke OnEnqueue event");
                    OnEnqueue(itemToBeCopied);
                }
            }
            if (count > 0)
            {
                _logger.Log(LogType.Log, "AHMAAAAAAAAAAAAAAAAAAAAAAAAAAAD",
                    String.Format("{0}/{1} actions has been copied.", count, count + pushOnlyItemsCount));
            }
        }


    }
}