using System;
using System.Collections.Generic;
using System.Text;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.src.Tasks;

namespace Automate.Model.Tasks
{
    public class Task
    {
        public Guid Guid { get; } = Guid.NewGuid();
        private Guid _assignedToGuid;
        public bool IsAssigned { get; private set; }
        private List<TaskAction> _taskActionList = new List<TaskAction>();
        private int _taskActionNumber = 0;
        //private IEnumerable<TaskAction> _taskActionList = new List<TaskAction>();

        internal Task()
        {
            
        }

        public Guid AssignedToGuid
        {
            get
            {
                if (!IsAssigned)
                    throw new TaskActionException("is not assigned, cannot get assigned GUID");
                return _assignedToGuid;
            }
            internal set
            {
                IsAssigned = true;
                _assignedToGuid = value;
            }
        }

        public bool IsPositionChangeRequiredForCurrentAction(Coordinate currentPosition)
        {
            if (IsTaskComplete())
                return false;
            return !_taskActionList[_taskActionNumber].TaskLocation.Equals(currentPosition);
        }

        public TaskAction GetCurrentAction()
        {
            if (IsTaskComplete())
                throw new TaskActionException("Task is complete, cannot get current action!");
           return _taskActionList[_taskActionNumber];
        }

        public void CommitActionAndMoveTaskToNextAction()
        {
            if (IsTaskComplete())
                throw new TaskActionException("Task is complete, cannot move to next action!");
            switch (GetCurrentAction().TaskActionType)
            {
                case TaskActionType.DeliveryTask:
                    TransportTaskAction deliveryAction = GetCurrentAction() as TransportTaskAction;
                    deliveryAction?.ComponentStack.DeliverAmount(Guid, deliveryAction.Amount);
                    break;
               case TaskActionType.PickupTask:
                    TransportTaskAction pickupAction = GetCurrentAction() as TransportTaskAction;
                    pickupAction?.ComponentStack.DeliverAmount(Guid, pickupAction.Amount);
                    break;
                default:
                    break;
            }


            _taskActionNumber++;
        }

        public void AddAction(TaskActionType taskActionType, Coordinate taskLocation, int amount)
        {
//            if ((taskActionType == TaskActionType.PickupTask) || (taskActionType == TaskActionType.DeliveryTask))
//                throw new TaskActionException("Use AddTransportAction for these types of actions");
            _taskActionList.Add(new TaskAction(Guid, taskLocation, taskActionType, amount));
        }

        public void AddTransportAction(TaskActionType taskActionType, Coordinate taskLocation, ComponentStack componentStack, int amount)
        {
            if (taskActionType == TaskActionType.PickupTask)
            {
                componentStack.AssignOutgoingAmount(Guid, amount);
            }
            if (taskActionType == TaskActionType.DeliveryTask)
            {
                /*to check if is full*/
                componentStack.AssignIncomingAmount(Guid, amount);
            }

            _taskActionList.Add(new TransportTaskAction(Guid, componentStack, taskLocation, taskActionType, amount));
        }

        public bool IsTaskComplete()
        {
            return _taskActionList.Count == _taskActionNumber;
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Task task = (Task)obj;
            return (Guid == task.Guid);
        }

        public override int GetHashCode() {
            return Guid.GetHashCode();
        }
    }
}