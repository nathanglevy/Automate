using System;
using System.CodeDom.Compiler;
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
    public class Scheduler<MasterAction> : IScheduler<MasterAction>
    {
        private Queue<MasterAction> _targetPushQ = new Queue<MasterAction>();
        private Queue<MasterAction> _targetPullQ = new Queue<MasterAction>();
        private AutoResetEvent _copyActionsAutoResetEvent = new AutoResetEvent(false);
        private Object _targetQLock = new Object();
        private event HandlerResultListner<MasterAction> _enqueueListener;
        public event NotifyOnPull<MasterAction> OnPull;
        public event NotifyOnPull<MasterAction> OnEnqueue;


        public Scheduler()
        {
            _enqueueListener += PushResultsToQueue;
            //_logger.logEnabled = true;
        }

      
        private void PushResultsToQueue(IHandlerResult<MasterAction> handlerResult)
        {
            Enqueue(handlerResult.GetItems());
        }

        public int ItemsCount
        {
            get
            {
                lock (_targetQLock)
                {
                    return _targetPullQ.Count;
                }
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
            get
            {
                lock (_targetQLock)
                {
                    return _targetPullQ.Count != 0;
                }
            }
        }

        public MasterAction Pull()
        {
            lock (_targetQLock)
            {
                if (HasItems)
                {

                    MasterAction deqMasterAction = _targetPullQ.Dequeue();

                    if (OnPull != null)
                        OnPull(deqMasterAction);

                    return deqMasterAction;

                }
                throw new Exception("Queue is empty, cannot pull");
            }

        }

        public HandlerResultListner<MasterAction> GetPushInvoker()
        {
            return _enqueueListener;
        }

        public void Enqueue(MasterAction item)
        {
            lock (_targetQLock)
            {
                _targetPushQ.Enqueue(item);
            }
            if (OnEnqueue != null)
                OnEnqueue(item);
        }

        public void OnPullFinish(ViewUpdateArgs args)
        {
           
        }

        public void OnPullStart(ViewUpdateArgs args)
        {
            // SWAP Between Pull and Push
            lock (_targetQLock)
            {
                // move all items under push Q to pull Q
                while (_targetPushQ.Count > 0)
                {
                    _targetPullQ.Enqueue(_targetPushQ.Dequeue());
                }
            }

        }

       
    }
}