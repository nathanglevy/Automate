using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.src.Controller.Interfaces;
using JetBrains.Annotations;

namespace Assets.src.Controller
{
    public class PrimaryObserver : IPrimaryObserver
    {
        [NotNull] private readonly List<ISecondryObserver> _observers;

        public PrimaryObserver()
        {
            _observers = new List<ISecondryObserver>();

        }

        public int GetObserversCount()
        {
            return _observers.Count;
        }

        public void Invoke<U>(U invokeArgs) where U : IObserverArgs
        {
            foreach (var observer in _observers)
            {
                observer.Notify(invokeArgs);
            }
        }

        public void RegisterObserver(ISecondryObserver secondryObserver)
        {
            _observers.Add(secondryObserver);
        }
    }
}