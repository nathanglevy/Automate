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
        //private ConcurrentQueue<MasterAction> _queue = new ConcurrentQueue<MasterAction>();
        private Queue<MasterAction> _queue = new Queue<MasterAction>();

        private Object _lock = new Object();


        // events
        private event HandlerResultListner<MasterAction> _enqueueListener;

        public Scheduler()
        {
            _enqueueListener += PushResultsToQueue;
        }

        private void PushResultsToQueue(IHandlerResult<MasterAction> handlerResult)
        {
//            var thread = new Thread(delegate()
//                {
                    Enqueue(handlerResult.GetItems());
//                }
//            );
//            thread.Start();
        }

        public int ItemsCount
        {
            get
            {
                return _queue.Count;
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
            //get { return !_queue.IsEmpty; }
            get { return _queue.Count != 0; }
        }

        public MasterAction Pull()
        {
            lock (_lock)
            {
                if (HasItems)
                {
                    
                    var masterAction = _queue.Dequeue();

                    Console.Out.WriteLine("Dequeued an item from Q, ID:" + masterAction.ToString());

                    // Fire OnPull To All Listners
                    if (OnPull != null) 
                        OnPull(masterAction);

                    // return to requestor
                    return masterAction;
                }
            }

            throw new Exception("Queue is empty, cannot pull");

        }

        public HandlerResultListner<MasterAction> GetPushInvoker()
        {
            return _enqueueListener;
        }

        public void Enqueue(MasterAction item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);
            }
        }

        public event NotifyOnPull<MasterAction> OnPull;
    }
}