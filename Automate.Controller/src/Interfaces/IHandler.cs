using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;

namespace Automate.Controller.Interfaces
{
    public interface IHandler
    {
        string Name { get; }
        //bool CanAcknowledge(MasterAction action);
    }

    public interface IHandler<T> : IHandler where T : IObserverArgs
    {
        bool CanHandle(T args);
        IHandlerResult<MasterAction> Handle(T args, IHandlerUtils utils);
        //IAcknowledgeResult<MasterAction> Acknowledge(MasterAction action, IHandlerUtils utils);
    }

    public abstract class Handler<T> : IHandler<T> where T:IObserverArgs
    {
        public string Name { get; private set; }

        public abstract IHandlerResult<MasterAction> Handle(T args, IHandlerUtils utils);

        //public virtual IAcknowledgeResult<MasterAction> Acknowledge(MasterAction action, IHandlerUtils utils)
        //{
        //    return null;
        //}

        //public virtual bool CanAcknowledge(MasterAction action)
        //{
        //    return false;
        //}
        public abstract bool CanHandle(T args);


     
    }

}