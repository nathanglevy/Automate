using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using Assets.src.PathFinding.MapModelComponents;

namespace Assets.src.Controller
{

    public class MockHandler : IHandler
    {

        public List<MasterAction> Actions { get; private set; }

//        public List<MasterAction> Handle<T>(T args,IHandleUtils utils) where T : IObserverArgs
        public List<MasterAction> Handle<T>(T args) where T : IObserverArgs
        {
            var selectionArgs = args as SelectionArgs;
//            IEnumerable<CellInfo> cells = utils.Model.GetCellsInRange(selectionArgs.UpperLeft, selectionArgs.LowerRight);

//            foreach (var cellInfo in cells)
//            {
//                if (cellInfo.IsPassable())
//                {
////                    utils.Model.revertPassableState(cellInfo);
//                }
//            }
            
            var action = new MockMasterAction(ActionType.AreaSelection,args.Id);
            var actions = new List<MasterAction>();
            actions.Add(action);
            Actions = actions;
            return actions;
        }

        public bool CanHandle<T>(T args) where T : IObserverArgs
        {
            return true;
        }
    }

    public interface IHandleUtils
    {
        IGameModel Model { get; }
    }
}