using System;
using System.Collections.Generic;
using System.Resources;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller.Modules
{
    public class SelectionHandler : IHandler
    {
        public List<MasterAction> Handle<T>(T args) where T : IObserverArgs
        {
            if (!isApplicable(args))
            {
                return new List<MasterAction>();
            }
            SelectionArgs selectionArgs = args as SelectionArgs;
            return null;
        }

        public bool isApplicable<T>(T args) where T : IObserverArgs
        {
            return args is SelectionArgs;
        }
    }
}