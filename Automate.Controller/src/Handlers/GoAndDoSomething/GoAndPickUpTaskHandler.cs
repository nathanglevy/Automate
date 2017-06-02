using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndDoSomething;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class GoAndPickUpTaskHandler : GoAndDoSomethingHandler
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            return base.Handle(args, utils);
        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");

            return args is GoAndPickUpAction;
        }

        protected override void OnMovableAtTargetDest(ControllerNotificationArgs args)
        {
            if (args.Utils == null)
                throw new ArgumentNullException("Utils Is Null, cannot get GameWorld ID.");

            var modelAction = args.Args as ModelMasterAction;
            var goAndPickUpAction = GetGoAndDoActionByMasterId(modelAction);

            var gameWorld = GameUniverse.GetGameWorldItemById(args.Utils.GameWorldId);
            var movableItem = gameWorld.GetMovable(goAndPickUpAction.MovableGuid);

            var pickUpAction = new PickUpAction(goAndPickUpAction.ComponentType, GetComponentCoordinate(goAndPickUpAction, movableItem), goAndPickUpAction.Amount, goAndPickUpAction.MovableGuid)
            {
                OnCompleteDelegate = AcknowledgeGoAndDoIsOver,
                Duration = new TimeSpan(0),
                NeedAcknowledge = false,
                MasterTaskId = modelAction.MasterTaskId,
            };
            args.Utils.InvokeHandler(pickUpAction);
        }

        protected override void AssignComponentStack(ComponentStack targetComponentStack, GoAndDoSomethingAction goAndDeliverAction, IMovable movable)
        {
            // TODO: REVISIT THIS AGAIN - ASSIGN_OUT_GOING OCCURS AT REQUIRMENTS HANDLER
           // targetComponentStack.AssignOutgoingAmount(goAndDeliverAction.MovableGuid, goAndDeliverAction.Amount);
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