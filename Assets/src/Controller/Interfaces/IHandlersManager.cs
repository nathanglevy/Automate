using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller
{
    public interface IHandlersManager
    {
        int GetHandlersCount();
        void AddHandler(IHandler handler);
//        List<MasterAction> Handle<T>(T args) where T:IObserverArgs;
        List<MasterAction> Handle(IObserverArgs args);
    }
}