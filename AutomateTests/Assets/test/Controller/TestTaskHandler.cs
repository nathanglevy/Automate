using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.TaskActionHandler;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestTaskHandler
    {
        private bool _handleTaskActionFired;
        private bool _taskIsCompleteFired;
        private AutoResetEvent _onCompletefireSync = new AutoResetEvent(false);
        private AutoResetEvent _handleMimicSync = new AutoResetEvent(false);

        [TestMethod]
        public void CreateNew_ShouldPass()
        {
            var taskHandler = new TaskHandler();
            Assert.IsNotNull(taskHandler);
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            var taskHandler = new TaskHandler();
            Assert.IsTrue(taskHandler.CanHandle(new TaskContainer(null)));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgs_ExpectFalse()
        {
            var taskHandler = new TaskHandler();
            Assert.IsFalse(taskHandler.CanHandle(new MoveAction(null,null,Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNull_ExpectException()
        {
            var taskHandler = new TaskHandler();
            taskHandler.CanHandle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleWithIncorrectArgs_ExpectException()
        {
            var taskHandler = new TaskHandler();
            taskHandler.Handle(new MoveAction(null, null, Guid.Empty), null);
        }

        [TestMethod]
        public void TestHandle_ExpectOnComplete()
        {
            var taskHandler = new TaskHandler();

            // build model, targetTask and Action
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(5, 5, 1));
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            newTask.AddAction(TaskActionType.PickupTask, new Coordinate(0, 1, 0), 100);

            // Handle the TargetTask
            // Expects --> to Handle Current Action
            var taskContainer = new TaskContainer(newTask);
            var handlerResult = taskHandler.Handle(taskContainer,new HandlerUtils(gameWorldItem.Guid,HandleMimic,null));

            taskContainer.OnComplete += TaskIsCompleted;

            

            _handleMimicSync.WaitOne(300);
            Assert.IsTrue(_handleTaskActionFired);

            _onCompletefireSync.WaitOne(300);
            Assert.IsTrue(_taskIsCompleteFired);
        }

        [TestMethod]
        public void TestAssignTaskToAMovable_ExpectOnlyMovableAssigned()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(3, 3, 0), MovableType.FastHuman);


        }

        private void TaskIsCompleted(ControllerNotificationArgs args)
        {
            _taskIsCompleteFired = true;
            _onCompletefireSync.Set();
        }

        private IList<ThreadInfo> HandleMimic(IObserverArgs args)
        {
            _handleTaskActionFired = args is TaskActionContainer;
            if (_handleTaskActionFired)
            {
                var taskActionContainer = args as TaskActionContainer;
                //taskActionContainer.TargetAction.
            }
            _handleMimicSync.Set();
            return null;
        }
    }

    public class TaskContainer : ModelMasterAction
    {
        public Task TargetTask { get; }

        public TaskContainer(Task targetTask) : base(ActionType.DEFAULT)
        {
            TargetTask = targetTask;
        }


        public Guid TargetId { get; }
        public event ControllerNotification OnComplete;


    }

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
            var taskActionContainer = new TaskActionContainer(currentAction);
            taskActionContainer.OnCompleteDelegate = UpdateAndHandleNextAction;

            return new HandlerResult(new List<MasterAction>() {taskActionContainer}){IsInternal = true};
        }

        private void UpdateAndHandleNextAction(ControllerNotificationArgs args)
        {
            var taskActionContainer = args.Args as TaskActionContainer;
            var masterTaskId = taskActionContainer.MasterTaskId;

            var gameWorldItem = GameUniverse.GetGameWorldItemById(args.Utils.GameWorldId);
            var targetTask = gameWorldItem.TaskDelegator.GetTaskByGuid(masterTaskId);

            targetTask.CommitActionAndMoveTaskToNextAction();

            if (targetTask.IsTaskComplete())
            {
                var taskContainer = _tasks[targetTask.Guid];
                taskContainer.OnCompleteDelegate.Invoke(new ControllerNotificationArgs(taskContainer));
            }
            else
            {
                var currentAction = targetTask.GetCurrentAction();
                var newTaskActionContainer = new TaskActionContainer(currentAction);
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
