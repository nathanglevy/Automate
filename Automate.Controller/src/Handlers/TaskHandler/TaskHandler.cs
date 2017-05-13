using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.TaskActionHandler;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;

namespace Automate.Controller.Handlers.TaskHandler
{
    public class TaskHandler : Handler<IObserverArgs>
    {

        Dictionary<Guid,TaskContainer> _tasks = new Dictionary<Guid, TaskContainer>();

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("Args is not expected, Handler Expects TaskContainer");

            // Safe Cast
            var taskContainer = args as TaskContainer;

            // Safe the Task for future usage
            _tasks.Add(taskContainer.TargetTask.Guid,taskContainer);

            // Get Original Task
            var task = taskContainer.TargetTask;

            // Get Current Action
            var currentAction = task.GetCurrentAction();

            // Create ActionContainer and Fire it
            var taskActionContainer = new TaskActionContainer(currentAction, task.Guid);
            taskActionContainer.OnCompleteDelegate = UpdateAndHandleNextAction;

            return new HandlerResult(new List<MasterAction>() {taskActionContainer}){IsInternal = true};
        }

        private void UpdateAndHandleNextAction(ControllerNotificationArgs args)
        {
            var taskActionContainer = args.Args as ModelMasterAction;
            var masterTaskId = taskActionContainer.MasterTaskId;

            var gameWorldItem = GameUniverse.GetGameWorldItemById(args.Utils.GameWorldId);
            var targetTask = gameWorldItem.TaskDelegator.GetTaskByGuid(masterTaskId);

            targetTask.CommitActionAndMoveTaskToNextAction();

            if (targetTask.IsTaskComplete())
            {

                var taskContainer = _tasks[targetTask.Guid];
                taskContainer.OnCompleteDelegate?.Invoke(
                    new ControllerNotificationArgs(taskContainer, utils: args.Utils));

                // Add Testing for this line
                gameWorldItem.TaskDelegator.RemoveCompletedTasks(targetTask.AssignedToGuid);
            }
            else
            {
                var currentAction = targetTask.GetCurrentAction();
                var newTaskActionContainer = new TaskActionContainer(currentAction, targetTask.Guid);
                newTaskActionContainer.OnCompleteDelegate = UpdateAndHandleNextAction;
                args.Utils.InvokeHandler(newTaskActionContainer);
            }
        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");
            return args is TaskContainer;
        }
    }
}