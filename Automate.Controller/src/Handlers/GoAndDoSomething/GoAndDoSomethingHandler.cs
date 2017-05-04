using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
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

            // Cast to Parent Class
            var goAndDoSomethingAction = args as GoAndDoSomethingAction;

            // Get the World Item
            var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);

            // TODO: Check With Naph, if Searching a non existing movable, what to expect
            var movableItem = gameWorldItem.GetMovable(goAndDoSomethingAction.MovableGuid);

            // Get the target component Stack
            var componentCoordinate = GetComponentCoordinate(goAndDoSomethingAction, movableItem);
            var componentStackGroup = gameWorldItem.GetComponentStackGroupAtCoordinate(componentCoordinate);
            var targetComponentStack = componentStackGroup.GetComponentStack(goAndDoSomethingAction.ComponentType);

            // Let Child Assign Incoming/OutComing Amount
            AssignComponentStack(targetComponentStack, goAndDoSomethingAction, movableItem);

            // Create the First Move Action To Dest
            var moveAction = new StartMoveAction(GetGoDestination(goAndDoSomethingAction, movableItem), movableItem.Guid)
            {
                OnCompleteDelegate = OnMovableAtTargetDest,
            };

            // Save the Action according to MasterTaskId
            if (_goAndPickActionsdict.ContainsKey(moveAction.MasterTaskId))
            {
                _goAndPickActionsdict[moveAction.MasterTaskId] =  goAndDoSomethingAction;
            }
            else
            {
                _goAndPickActionsdict.Add(moveAction.MasterTaskId, goAndDoSomethingAction);
            }
            
            // Return MoveAction as Internal to be handled by MoveHandler
            return new HandlerResult(new List<MasterAction>() {moveAction}) {IsInternal = true};

        }


        /// <summary>
        /// Method gets the action to be handled and return true/false if it can handle it
        /// </summary>
        /// <param name="args"></param>
        /// <returns>true, if handler can handle such action</returns>
        public abstract override bool CanHandle(IObserverArgs args);

        /// <summary>
        /// abstract method which determine the TARGET destination to go and perform the Do
        /// </summary>
        /// <param name="goAndDoSomethingAction"></param>
        /// <param name="movableItem"></param>
        /// <returns>Coordinate of Target</returns>
        protected abstract Coordinate GetGoDestination(GoAndDoSomethingAction goAndDoSomethingAction, IMovable movableItem);

        /// <summary>
        /// Method which called after movable gets to it's target, at this method we need to implement the "DO"
        /// </summary>
        /// <param name="args"></param>
        protected abstract void OnMovableAtTargetDest(ControllerNotificationArgs args);

        /// <summary>
        /// the method returns the coordinate of the component to perform the Do on
        /// </summary>
        /// <param name="goAndDoSomethingAction"></param>
        /// <param name="movableItem"></param>
        /// <returns></returns>
        protected abstract Coordinate GetComponentCoordinate(GoAndDoSomethingAction goAndDoSomethingAction, IMovable movableItem);

        /// <summary>
        /// method let the derived class Assign incming/outcoming and any on the target compoenent stack
        /// </summary>
        /// <param name="targetComponentStack"></param>
        /// <param name="goAndDeliverAction"></param>
        /// <param name="movable"></param>
        protected abstract void AssignComponentStack(ComponentStack targetComponentStack, GoAndDoSomethingAction goAndDeliverAction, IMovable movable);


        /// <summary>
        /// Last Action To Be called, default implmentation fire the OnComplete Event
        /// </summary>
        /// <param name="args"></param>
        protected void AcknowledgeGoAndDoIsOver(ControllerNotificationArgs args)
        {
            var modelAction = args.Args as ModelMasterAction;
            var goAndPickUpAction = GetGoAndDoActionByMasterId(modelAction);
            goAndPickUpAction.FireOnComplete(new ControllerNotificationArgs(goAndPickUpAction, utils:args.Utils));
        }

        // Help Methods
        protected GoAndDoSomethingAction GetGoAndDoActionByMasterId(ModelMasterAction modelAction)
        {
            return _goAndPickActionsdict[modelAction.MasterTaskId];
        }

      

    }
}