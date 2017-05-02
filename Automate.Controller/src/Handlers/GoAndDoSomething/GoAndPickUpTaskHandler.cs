using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndDoSomething;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class GoAndPickUpTaskHandler : GoAndDoSomethingHandler
    {


        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");

            return args is GoAndPickUpAction;
        }

        protected override void OnMovableAtTargetDest(ControllerNotificationArgs args)
        {
            var modelAction = args.Args as ModelMasterAction;
            var goAndPickUpAction = GetGoAndDoActionByMasterId(modelAction);

            var gameWorld = GameUniverse.GetGameWorldItemById(args.Utils.GameWorldId);
            var movableItem = gameWorld.GetMovable(goAndPickUpAction.MovableGuid);

            var pickUpAction = new PickUpAction(ComponentType.IronOre, GetComponentCoordinate(goAndPickUpAction, movableItem), goAndPickUpAction.Amount, goAndPickUpAction.MovableGuid)
            {
                OnCompleteDelegate = AcknowledgeGoAndDoIsOver,
                Duration = new TimeSpan(0),
                NeedAcknowledge = false,
            };
            args.Utils.InvokeHandler(pickUpAction);
        }

        protected override void AssignComponentStack(ComponentStack targetComponentStack, GoAndDoSomethingAction goAndDeliverAction, IMovable movable)
        {
            targetComponentStack.AssignOutgoingAmount(goAndDeliverAction.MovableGuid, goAndDeliverAction.Amount);
        }

        protected override Coordinate GetGoDestination(GoAndDoSomethingAction goAndDoSomethingAction, IMovable movableItem)
        {
            return goAndDoSomethingAction.TargetDestCoordinate;
        }

        protected override Coordinate GetComponentCoordinate(GoAndDoSomethingAction goAndDoSomethingAction, IMovable movableItem)
        {
            return goAndDoSomethingAction.TargetDestCoordinate;
        }

    }
}