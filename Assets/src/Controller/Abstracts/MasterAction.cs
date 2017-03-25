using System;
using Assets.src.Controller.Interfaces;
using UnityEditor;

namespace Assets.src.Controller.Abstracts
{
    public class MasterAction : IObserverArgs
    {
        private string _targetId;

        public MasterAction(ActionType type, string targetId)
        {
            Type = type;
            _targetId = targetId;
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