using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public class GoAndDeliverTaskHandler : GoAndDoSomethingHandler    
    {
        protected override Coordinate GetGoDestination(GoAndDoSomethingAction goAndDoSomethingAction, IMovable movableItem)
        {
            return goAndDoSomethingAction.TargetDestCoordinate;
        }

        protected override void OnMovableAtTargetDest(ControllerNotificationArgs args)
        {
            var modelAction = args.Args as ModelMasterAction;
            var goAndDeliver = GetGoAndDoActionByMasterId(modelAction);

            var gameWorld = GameUniverse.GetGameWorldItemById(args.Utils.GameWorldId);
            var movableItem = gameWorld.GetMovable(goAndDeliver.MovableGuid);

            try
            {
                var movableCSG = movableItem.ComponentStackGroup;
                var componentStack = movableCSG.GetComponentStack(goAndDeliver.ComponentType);

            }
            catch (Exception e)
            {
                throw new ComponentStackAssignError(e.ToString());
            }
            

            var deliverAction = new DeliverAction(goAndDeliver.ComponentType, GetGoDestination(goAndDeliver,movableItem), GetComponentCoordinate(goAndDeliver,movableItem), goAndDeliver.Amount, goAndDeliver.MovableGuid)
            {
                OnCompleteDelegate = AcknowledgeGoAndDoIsOver,
                Duration = new TimeSpan(0),
                NeedAcknowledge = false,
            };
            args.Utils.InvokeHandler(deliverAction);
        }

        protected override Coordinate GetComponentCoordinate(GoAndDoSomethingAction goAndDoSomethingAction, IMovable movableItem)
        {
            return goAndDoSomethingAction.TargetDestCoordinate;
        }

        protected override void AssignComponentStack(ComponentStack targetComponentStack, GoAndDoSomethingAction goAndDeliverAction, IMovable movable)
        {
            // Assign Incoming to Target Component Stack
            targetComponentStack.AssignIncomingAmount(goAndDeliverAction.MovableGuid, goAndDeliverAction.Amount);

            // Assign OutComing to Movable
            var movableComponentStack = movable.ComponentStackGroup.GetComponentStack(targetComponentStack.ComponentType);
            movableComponentStack.AssignOutgoingAmount(movable.Guid,goAndDeliverAction.Amount);
        }



        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");

            return args is GoAndDeliverAction;
        }
    }

    public class ComponentStackAssignError : Exception
    {
        public ComponentStackAssignError(string toString) : base(toString)
        {
        }
    }
}