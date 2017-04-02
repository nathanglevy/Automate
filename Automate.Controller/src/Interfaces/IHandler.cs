using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;

namespace Automate.Controller.Interfaces
{
    public interface IHandler
    {
        string Name { get; }
//        Type GetInputType();
        bool CanAcknowledge(MasterAction action);
    }

    public interface IHandler<T> : IHandler where T : IObserverArgs
    {
        bool CanHandle(T args);
        IHandlerResult Handle(T args, IHandlerUtils utils);
        IAcknowledgeResult Acknowledge(MasterAction action,IHandlerUtils utils);
    }

    public abstract class Handler<T> : IHandler<T> where T:IObserverArgs
    {
        public string Name { get; private set; }

        public abstract IHandlerResult Handle(T args, IHandlerUtils utils);
        public abstract IAcknowledgeResult Acknowledge(MasterAction action, IHandlerUtils utils);
        public abstract bool CanAcknowledge(MasterAction action);
        public abstract bool CanHandle(T args);

        protected virtual Type GetInputType()
        {
            throw new NotImplementedException();
        }

        


     
    }

}