using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class GoAndPickUpTaskHandler : Handler<IObserverArgs>, IHandler<IObserverArgs>
    {
        private Dictionary<Guid, GoAndPickUpAction> _goAndPickActionsdict =   new Dictionary<Guid,GoAndPickUpAction>();

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException(
                    "Args is null, therfore, cannot determine if Handler should be activated");
            return args is GoAndPickUpAction;
        }

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException(
                    "Args is not of type GoAndPickUpAction, you should call CanHandle Before Calling Handle");

            var goAndPickUpAction = args as GoAndPickUpAction;
            var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);

            // TODO: Check With Naph, if Searching a non existing movable, what to expect
            var movableItem = gameWorldItem.GetMovableItem(goAndPickUpAction.MovableGuid);

            // let's allocate the outgoing amount
            var targetCombo = gameWorldItem.GetComponentsAtCoordinate(goAndPickUpAction.TargetDestCoordinate);
            targetCombo.AssignOutgoingAmount(goAndPickUpAction.MovableGuid,goAndPickUpAction.Amount);

            var moveAction = new StartMoveAction(goAndPickUpAction.TargetDestCoordinate, movableItem.CurrentCoordiate, movableItem.Guid)
            {
                OnCompleteDelegate = OnMovableAtTargetDest,
            };

            _goAndPickActionsdict.Add(moveAction.MasterTaskId, goAndPickUpAction);

            utils.InvokeHandler(moveAction);
            return new HandlerResult(new List<MasterAction>());

        }

        private void OnMovableAtTargetDest(ControllerNotificationArgs args)
        {
            var modelAction = args.Args as ModelMasterAction;
            var goAndPickUpAction = _goAndPickActionsdict[modelAction.MasterTaskId];

            var pickUpAction = new PickUpAction(goAndPickUpAction.TargetDestCoordinate, goAndPickUpAction.Amount,goAndPickUpAction.MovableGuid )
            {
                OnCompleteDelegate = AcknowledgeGoAndPickIsOver,
                Duration = new TimeSpan(0),
                NeedAcknowledge = false,
            };
            args.Utils.InvokeHandler(pickUpAction);
        }

        private void AcknowledgeGoAndPickIsOver(ControllerNotificationArgs args)
        {
            var modelAction = args.Args as ModelMasterAction;
            var goAndPickUpAction = _goAndPickActionsdict[modelAction.MasterTaskId];
            goAndPickUpAction.FireOnComplete(new ControllerNotificationArgs(goAndPickUpAction) {Utils = args.Utils});
        }

    }
}