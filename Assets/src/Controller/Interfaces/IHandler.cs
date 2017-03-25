using System;
using System.Collections;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Handlers;
using Assets.src.Controller.Handlers.SelectionNotification;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller
{
    public interface IHandler
    {
        string Name { get; }
        Type GetInputType();
    }

    public interface IHandler<T> : IHandler where T : IObserverArgs
    {
        bool CanHandle(T args);
        void Handle(T args, IHandlerUtils utils);
//        bool CanHandle<T>(T args) where T : ObserverArgs;
//        void Handle<T>(T args, IHandlerUtils utils) where T : ObserverArgs;
    }

    public abstract class Handler<T> : IHandler<T> where T:IObserverArgs
    {
        public string Name { get; private set; }

        public virtual bool CanHandle(T args)
        {
            return true;
        }

        public abstract void Handle(T args, IHandlerUtils utils);
        public Type GetInputType()
        {
            throw new NotImplementedException();
        }
    }

}