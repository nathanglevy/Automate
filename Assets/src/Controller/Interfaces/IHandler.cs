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
        IHandlerResult Handle(T args, IHandlerUtils utils);
    }

    public abstract class Handler<T> : IHandler<T> where T:IObserverArgs
    {
        public string Name { get; private set; }

        public virtual bool CanHandle(T args)
        {
            return true;
        }

        public abstract IHandlerResult Handle(T args, IHandlerUtils utils);
        public Type GetInputType()
        {
            throw new NotImplementedException();
        }
    }

}