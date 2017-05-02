using System;
using System.Collections.Generic;
using System.Text;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks
{
    public class Task
    {
        public Guid Guid { get; } = Guid.NewGuid();
        private Guid _assignedToGuid;
        public bool IsAssigned { get; private set; }
        private List<ITaskAction> _taskActionList = new List<ITaskAction>();
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
            set
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

        public ITaskAction GetCurrentAction()
        {
            if (IsTaskComplete())
                throw new TaskActionException("Task is complete, cannot get current action!");
           return _taskActionList[_taskActionNumber];
        }

        public void CommitActionAndMoveTaskToNextAction()
        {
            if (IsTaskComplete())
                throw new TaskActionException("Task is complete, cannot move to next action!");
            GetCurrentAction().OnCompleted();
            /*switch (GetCurrentAction().TaskActionType)
            {
                case TaskActionType.DeliveryTask:
                    DeliverTaskAction deliveryAction = GetCurrentAction() as DeliverTaskAction;
                    break;
               case TaskActionType.PickupTask:
                    DeliverTaskAction pickupAction = GetCurrentAction() as DeliverTaskAction;
                    break;
                default:
                    break;
            }*/
            _taskActionNumber++;
        }

        public ITaskAction AddAction(Coordinate taskLocation, int amount)
        {
            //            if ((taskActionType == TaskActionType.PickupTask) || (taskActionType == TaskActionType.DeliveryTask))
            //                throw new TaskActionException("Use AddTransportAction for these types of actions");
            ITaskAction newAction = new TaskAction(Guid, taskLocation, amount);
            _taskActionList.Add(newAction);
            return newAction;
        }

        public ITaskAction AddTransportAction(TaskActionType taskActionType, Coordinate taskLocation, ComponentStackGroup componentStackGroup, Component component, int amount)
        {
            if (taskActionType == TaskActionType.PickupTask)
            {
                ITaskAction pickupTaskAction = new PickupTaskAction(Guid, componentStackGroup, taskLocation, component, amount);
                _taskActionList.Add(pickupTaskAction);
                return pickupTaskAction;
            }
            if (taskActionType == TaskActionType.DeliveryTask)
            {
                ITaskAction deliverTaskAction = new DeliverTaskAction(Guid, componentStackGroup, taskLocation, component, amount);
                _taskActionList.Add(deliverTaskAction);
                return deliverTaskAction;
            }
            throw new TaskActionException("Give action type is not a transport type");

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

        public TaskAction AddAction(TaskActionType taskLocation, Coordinate coordinate, int amount)
        {
            throw new NotImplementedException("This method should no longer be used -- use AddAction generic, AddTransportAction, or AddWorkAction accordingly");
        }
    }
}