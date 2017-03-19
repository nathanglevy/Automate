using System;
using Assets.src.Controller.Interfaces;
using UnityEditor;

namespace Assets.src.Controller.Abstracts
{
    public class MasterAction : IObserverArgs
    {
        public MasterAction(ActionType type)
        {
            Type = type;
            Id = Guid.NewGuid().ToString();
        }

        public ActionType Type { get; protected set; }
        public string Id { get; protected set; }

    }
}