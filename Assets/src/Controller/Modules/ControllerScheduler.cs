using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller.Modules
{
    public class ControllerScheduler : IScheduler
    {
        private readonly IPrimaryObserver _target;
        private readonly Dictionary<string, MasterAction> _pipe;

        public ControllerScheduler(IPrimaryObserver target)
        {
            this._target = target;
            _pipe = new Dictionary<string, MasterAction>();
        }


        public int GetActionsCount()
        {
            return _pipe.Count;
        }

        public IPrimaryObserver getTarget()
        {
            return _target;
        }

        public void Schedule(List<MasterAction> actions)
        {
            foreach (var action in actions)
            {
                _target.Invoke(action);
                _pipe.Add(action.Id, action);
            }
        }

       
    }
}