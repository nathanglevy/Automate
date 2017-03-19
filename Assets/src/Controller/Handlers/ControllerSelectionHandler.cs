using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using Assets.src.PathFinding.MapModelComponents;

namespace Assets.src.Controller.Handlers
{
    public class ControllerSelectionHandler : IHandler
    {
        public bool isApplicable<T>(T args) where T : IObserverArgs
        {
            return args is SelectionArgs;
        }

        public List<MasterAction> Handle<T>(T args) where T : IObserverArgs
        {
            if (!isApplicable(args)) return new List<MasterAction>();
            SelectionArgs sArgs = args as SelectionArgs;
            var selectionAction = new SelectionMasterAction(sArgs.UpperLeft, sArgs.LowerRight);
            var masterActions = new List<MasterAction>();
            masterActions.Add(selectionAction);
            return masterActions;
        }
    }

}