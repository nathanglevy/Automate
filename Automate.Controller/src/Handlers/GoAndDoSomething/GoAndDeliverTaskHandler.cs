using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public class GoAndDeliverTaskHandler : GoAndDoSomethingHandler    
    {
        protected override Coordinate GetGoDestination(GoAndDoSomethingAction goAndDoSomethingAction, MovableItem movableItem)
        {
            return goAndDoSomethingAction.TargetDestCoordinate;
        }

        protected override void OnMovableAtTargetDest(ControllerNotificationArgs args)
        {
            var modelAction = args.Args as ModelMasterAction;
            var goAndDeliver = GetGoAndDoActionByMasterId(modelAction);

            var gameWorld = GameUniverse.GetGameWorldItemById(args.Utils.GameWorldId);
            var movableItem = gameWorld.GetMovableItem(goAndDeliver.MovableGuid);

            var deliverAction = new DeliverAction(GetGoDestination(goAndDeliver,movableItem),GetComponentCoordinate(goAndDeliver,movableItem), goAndDeliver.Amount, goAndDeliver.MovableGuid)
            {
                OnCompleteDelegate = AcknowledgeGoAndDoIsOver,
                Duration = new TimeSpan(0),
                NeedAcknowledge = false,
            };
            args.Utils.InvokeHandler(deliverAction);
        }

        protected override Coordinate GetComponentCoordinate(GoAndDoSomethingAction goAndDoSomethingAction, MovableItem movableItem)
        {
            // it should be at movable Current Coordinate
            // TODO: add check that movable Current has Component
            return goAndDoSomethingAction.TargetDestCoordinate;
        }

        protected override void AssignComponentStack(ComponentStack targetCombo, GoAndDoSomethingAction goAndPickUpAction)
        {
            targetCombo.AssignIncomingAmount(goAndPickUpAction.MovableGuid, goAndPickUpAction.Amount);
        }


        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");

            return args is GoAndDeliverAction;
        }
    }
}