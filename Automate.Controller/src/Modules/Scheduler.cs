using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Modules
{
    public class Scheduler<MasterAction> : IScheduler<MasterAction>
    {
        // collection
        //private ConcurrentQueue<MasterAction> _pushOnlyQueue = new ConcurrentQueue<MasterAction>();
        private Queue<MasterAction> _pushOnlyQueue = new Queue<MasterAction>();
        private Queue<MasterAction> _pullOnlyQueue = new Queue<MasterAction>();
        private AutoResetEvent _copyActionsAutoResetEvent = new AutoResetEvent(false);
        private Object _pushOnlyLock = new Object();
        private Object _pullOnlylock = new Object();
        private bool _copyThreadCanRun = true;
        private Thread _copyThread;
        // events
        private event HandlerResultListner<MasterAction> _enqueueListener;
        public event NotifyOnPull<MasterAction> OnPull;
        public event NotifyOnPull<MasterAction> OnEnqueue;


        public Scheduler()
        {
            _enqueueListener += PushResultsToQueue;
        }

        private void CopyToPullOnlyQ()
        {
            while (_pushOnlyQueue.Count > 0)
            {
                _copyActionsAutoResetEvent.WaitOne();
                MasterAction itemToBeCopied = default(MasterAction);
                lock (_pushOnlyLock)
                {
                    itemToBeCopied = _pushOnlyQueue.Dequeue();
                    lock (_pullOnlyQueue)
                    {
                        _pullOnlyQueue.Enqueue(itemToBeCopied);
                    }

                }
                if (OnEnqueue != null && itemToBeCopied != null)
                    OnEnqueue(itemToBeCopied);
            }
        }

        private void PushResultsToQueue(IHandlerResult<MasterAction> handlerResult)
        {
            Enqueue(handlerResult.GetItems());
        }

        public int ItemsCount
        {
            get
            {
                return _pushOnlyQueue.Count;
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
            get { return _pushOnlyQueue.Count != 0; }
        }

        public MasterAction Pull()
        {
            MasterAction deqMasterAction = default(MasterAction);
            lock (_pullOnlylock)
            {
                if (HasItems)
                {
                    deqMasterAction = _pushOnlyQueue.Dequeue();
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
                _pushOnlyQueue.Enqueue(item);
            }

        }

        public void OnPullFinish(ViewUpdateArgs args)
        {
            if (_copyThread == null || !_copyThread.IsAlive)
            {
                _copyThread = new Thread(CopyToPullOnlyQ);
                _copyThread.Priority = ThreadPriority.AboveNormal;
                _copyThread.Start();
            }
            _copyActionsAutoResetEvent.Set();
        }

        public void OnPullStart(ViewUpdateArgs args)
        {
            _copyActionsAutoResetEvent.Reset();
        }
    }
}