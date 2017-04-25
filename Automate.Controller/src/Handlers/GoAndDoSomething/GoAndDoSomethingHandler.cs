using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public abstract class GoAndDoSomethingHandler : Handler<IObserverArgs>, IHandler<IObserverArgs>
    {
        private Dictionary<Guid, GoAndDoSomethingAction> _goAndPickActionsdict =   new Dictionary<Guid, GoAndDoSomethingAction>();

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException(
                    "Args is not of type GoAndPickUpAction, you should call CanHandle Before Calling Handle");

            var goAndDoSomethingAction = args as GoAndDoSomethingAction;
            var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);

            // TODO: Check With Naph, if Searching a non existing movable, what to expect
            var movableItem = gameWorldItem.GetMovableItem(goAndDoSomethingAction.MovableGuid);

            // let's allocate the outgoing amount
            var targetCombo = gameWorldItem.GetComponentsAtCoordinate(GetComponentCoordinate(goAndDoSomethingAction, movableItem));
            AssignComponentStack(targetCombo, goAndDoSomethingAction);

            var moveAction = new StartMoveAction(GetGoDestination(goAndDoSomethingAction, movableItem), movableItem.Guid)
            {
                OnCompleteDelegate = OnMovableAtTargetDest,
            };

            if (_goAndPickActionsdict.ContainsKey(moveAction.MasterTaskId))
            {
                _goAndPickActionsdict[moveAction.MasterTaskId] =  goAndDoSomethingAction;
            }
            else
            {
                _goAndPickActionsdict.Add(moveAction.MasterTaskId, goAndDoSomethingAction);
            }
            

            return new HandlerResult(new List<MasterAction>() {moveAction}) {IsInternal = true};

        }

   


        public abstract override bool CanHandle(IObserverArgs args);
        protected abstract Coordinate GetGoDestination(GoAndDoSomethingAction goAndDoSomethingAction, MovableItem movableItem);
        protected abstract void OnMovableAtTargetDest(ControllerNotificationArgs args);
        protected abstract Coordinate GetComponentCoordinate(GoAndDoSomethingAction goAndDoSomethingAction, MovableItem movableItem);

        protected abstract void AssignComponentStack(ComponentStack targetCombo,
            GoAndDoSomethingAction goAndPickUpAction);


        protected void AcknowledgeGoAndDoIsOver(ControllerNotificationArgs args)
        {
            var modelAction = args.Args as ModelMasterAction;
            var goAndPickUpAction = GetGoAndDoActionByMasterId(modelAction);
            goAndPickUpAction.FireOnComplete(new ControllerNotificationArgs(goAndPickUpAction) { Utils = args.Utils });
        }


        // Help Methods
        protected GoAndDoSomethingAction GetGoAndDoActionByMasterId(ModelMasterAction modelAction)
        {
            return _goAndPickActionsdict[modelAction.MasterTaskId];
        }

      

    }
}