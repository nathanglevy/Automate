using System;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Abstracts
{
    public class MasterAction : IObserverArgs
    {
        private string _targetId;
        private readonly TimeSpan _duration;

        public TimeSpan Duration
        {
            get { return _duration; }
        }

        public MasterAction(ActionType type, string targetId)
        {
            Type = type;
            _targetId = targetId;
            _duration = new TimeSpan();
        }

        public MasterAction(ActionType type)
        {
            Type = type;
        }

        public ActionType Type { get; protected set; }

        public string TargetId
        {
            get { return _targetId; }
            set { _targetId = value; }
        }

    }
}