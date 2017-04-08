using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Modules
{
    public class Scheduler : IScheduler
    {
        // collection
        private Queue<MasterAction> _queue = new Queue<MasterAction>();

        // events
        private event HandlerResultListner _enqueueListener;

        public Scheduler()
        {
            _enqueueListener += PushResultsToQueue;
        }

        private void PushResultsToQueue(IHandlerResult handlerResult)
        {
            Enqueue(handlerResult.GetActions());
        }

        public int ActionsCount
        {
            get
            {
                return _queue.Count;
            }
        }

        public void Enqueue(IList<MasterAction> actions)
        {
            foreach (var action in actions)
            {
                _queue.Enqueue(action);
            }
        }

        public bool HasActions
        {
            get { return (_queue.Count > 0); }
        }

        public MasterAction Pull()
        {
            MasterAction action = null;
            bool actionPulled = false;

            while (!actionPulled && (_queue.Count > 0))
            {
                action = _queue.Dequeue();
            }

            return action;

        }

        public HandlerResultListner GetPushInvoker()
        {
            return _enqueueListener;
        }
    }
}