using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
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

    public class ThreadedPrimaryObserver : PrimaryObserver
    {
        public void Invoke<U>(U invokeArgs) where U : IObserverArgs
        {
            var thread = new Thread(delegate()
            {
                Console.Out.WriteLine("Before Invoking");
                base.Invoke(invokeArgs);
                Console.Out.WriteLine("Finished Invoking");
                
            });

            Console.Out.WriteLine(thread.Name + "STARTS NOW");
            thread.Start();
        }
    }
}