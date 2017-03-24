using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller
{
    public interface IHandler
    {
        bool isApplicable<T>(T args) where T : IObserverArgs;
        List<MasterAction> Handle<T>(T args) where T:IObserverArgs;
    }
}