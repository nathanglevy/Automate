using System;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Abstracts
{
    public abstract class ObserverArgs : IObserverArgs
    {
        public string TargetId { get;  set; }
        
    }
}