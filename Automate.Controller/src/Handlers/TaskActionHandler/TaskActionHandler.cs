using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.TaskActionHandler
{
    public class TaskActionHandler : Handler<IObserverArgs>
    {

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");
            return args is TaskActionContainer;
        }

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
          if (!CanHandle(args))
                throw new ArgumentException("Args must be TaskActionContainer, incorrect argument received");

            var taskActionContainer = args as TaskActionContainer;
            var taskGuid = taskActionContainer.MasterTaskId;
            var gameWorld = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
            var task = gameWorld.TaskDelegator.GetTaskByGuid(taskGuid);
            
            switch (taskActionContainer.TargetAction.TaskActionType)
            {
                case TaskActionType.PickupTask:
                    var goAndPickUpAction = new GoAndPickUpAction(taskActionContainer.TargetAction.TaskLocation,
                        taskActionContainer.TargetAction.Amount, task.AssignedToGuid)
                    {
                        OnCompleteDelegate = taskActionContainer.OnCompleteDelegate,
                        MasterTaskId = taskActionContainer.MasterTaskId
                    } ;
                    return new HandlerResult(new List<MasterAction>() {goAndPickUpAction}){ IsInternal = true };
                case TaskActionType.DeliveryTask:
                    var goAndDeliverAction = new GoAndDeliverAction(taskActionContainer.ComponentType,taskActionContainer.TargetAction.TaskLocation,
                        taskActionContainer.TargetAction.Amount, task.AssignedToGuid)
                    {
                        OnCompleteDelegate = taskActionContainer.OnCompleteDelegate,
                        MasterTaskId = taskActionContainer.MasterTaskId

                    };
                    return new HandlerResult(new List<MasterAction>() { goAndDeliverAction }) { IsInternal = true };
                case TaskActionType.WorkTask:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unkown TaskActionType");
            }
            return null;
        }

    }
}