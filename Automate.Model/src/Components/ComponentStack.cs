using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;

namespace Automate.Model.Components
{
    public class ComponentStack : ITaskAttachable
    {
        public Guid Guid { get; }
        public Component ComponentType { get; }
        public int CurrentAmount { get; private set; }
        public float CurrentTotalSpace => CurrentAmount * ComponentType.Size; 
        public float CurrentTotalWeight => CurrentAmount * ComponentType.Weight;
        public float TotalIncomingSpace => IncomingAllocatedAmount * ComponentType.Size; 
        public float TotalIncomingWeight => IncomingAllocatedAmount * ComponentType.Weight; 
        public int RemainingAmountForOutgoing => CurrentAmount - OutgoingAllocatedAmount;
        public int RemainingAmountForIncoming => MaxAmountInStack - CurrentAmount - IncomingAllocatedAmount;
        public int OutgoingAllocatedAmount => _outgoingAllocations.Values.Sum();
        public int IncomingAllocatedAmount => _incomingAllocations.Values.Sum();
        public int RemainingAmountForTaskOutgoing => CurrentAmount - OutgoingTaskAllocatedAmount;
        public int RemainingAmountForTaskIncoming => MaxAmountInStack - CurrentAmount - IncomingTaskAllocatedAmount;
        public int OutgoingTaskAllocatedAmount => _taskActions.Where(item => item.TaskActionType == TaskActionType.PickupTask).Sum(item => item.Amount);
        public int IncomingTaskAllocatedAmount => _taskActions.Where(item => item.TaskActionType == TaskActionType.DeliveryTask).Sum(item => item.Amount);
        public int StackMax { get; set; } = 2000;
        public int MaxAmountInStack => (int) (StackMax / ComponentType.Size);
        private Dictionary<Guid,int> _outgoingAllocations = new Dictionary<Guid, int>();
        private Dictionary<Guid,int> _incomingAllocations = new Dictionary<Guid, int>();
        private List<ITaskAction> _taskActions = new List<ITaskAction>();

        public ComponentStack(Component componentType, int currentAmount)
        {
            CheckPositiveValue(currentAmount);
            Guid = Guid.NewGuid();
            ComponentType = componentType;
            CurrentAmount = currentAmount;
        }

        public int AddAmount(int amount)
        {
            CheckPositiveValue(amount);
            if (amount + CurrentAmount > MaxAmountInStack)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot add the following amount: " + amount + ", currentAmount " + CurrentAmount + " would be higher than stack max: " + StackMax);

            return CurrentAmount += amount;
        }

        public int RemoveAmount(int amount)
        {
            CheckPositiveValue(amount);
            if (amount > CurrentAmount)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot remove the following amount: " + amount + ", currentAmount is higher than existing currentAmount: " + CurrentAmount);

            return CurrentAmount -= amount;
        }

        public void AssignOutgoingAmount(Guid targetGuid, int amount)
        {
            CheckPositiveValue(amount);
            if (RemainingAmountForOutgoing < amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot allocate outgoing more than existing currentAmount");

            if (_outgoingAllocations.ContainsKey(targetGuid))
                _outgoingAllocations[targetGuid] += amount;
            else
                _outgoingAllocations[targetGuid] = amount;
        }

        public void UnassignOutgoingAmount(Guid targetGuid, int amount) {
            CheckPositiveValue(amount);
            if (!_outgoingAllocations.ContainsKey(targetGuid))
                throw new ArgumentException("Target Guid does not have an outgoing assignment: " + targetGuid);
            if (_outgoingAllocations[targetGuid] < amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot unassign more than assigned currentAmount");

            if (_outgoingAllocations[targetGuid] == amount)
                _outgoingAllocations.Remove(targetGuid);
            else
                _outgoingAllocations[targetGuid] -= amount;
        }

        public void AssignIncomingAmount(Guid targetGuid, int amount)
        {
            CheckPositiveValue(amount);
            if (RemainingAmountForIncoming < amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot assign for incoming more than stack size");

            if (_incomingAllocations.ContainsKey(targetGuid))
                _incomingAllocations[targetGuid] += amount;
            else
                _incomingAllocations[targetGuid] = amount;
        }

        public void UnassignIncomingAmount(Guid targetGuid, int amount) {
            CheckPositiveValue(amount);
            if (!_incomingAllocations.ContainsKey(targetGuid))
                throw new ArgumentException("Target Guid does not have an outgoing assignment: " + targetGuid);
            if (_incomingAllocations[targetGuid] < amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot unassign more than assigned currentAmount");

            if (_incomingAllocations[targetGuid] == amount)
                _incomingAllocations.Remove(targetGuid);
            else
                _incomingAllocations[targetGuid] -= amount;
        }

        public void DeliverAmount(Guid targetGuid, int amount)
        {
            if (!_incomingAllocations.ContainsKey(targetGuid))
                throw new ArgumentException("delivering Guid: " + targetGuid + " does not have an awaiting slot in this stack");
            if (_incomingAllocations[targetGuid] < amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "delivering Guid: " + targetGuid +
                    " is trying to deliver " + (amount) + " components but stack is only awaiting " + _outgoingAllocations[targetGuid]);
            AddAmount(amount);
            UnassignIncomingAmount(targetGuid, amount);
        }

        public void PickupAmount(Guid targetGuid, int amount)
        {
            if (!_outgoingAllocations.ContainsKey(targetGuid))
                throw new ArgumentException("delivering Guid: " + targetGuid + " does not have an awaiting slot in this stack");
            if (_outgoingAllocations[targetGuid] < amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "delivering Guid: " + targetGuid +
                    " is trying to deliver " + (amount) + " components but stack is only awaiting " + _outgoingAllocations[targetGuid]);
            RemoveAmount(amount);
            UnassignOutgoingAmount(targetGuid, amount);
        }

        public int GetIncomingAllocatedAmountForGuid(Guid targetGuid)
        {
            if (!_incomingAllocations.ContainsKey(targetGuid))
                return 0;
            return _incomingAllocations[targetGuid];
        }

        public int GetOutgoingAllocatedAmountForGuid(Guid targetGuid) {
            if (!_outgoingAllocations.ContainsKey(targetGuid))
                return 0;
            return _outgoingAllocations[targetGuid];
        }

        private void CheckPositiveValue(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Method does not accept a negative currentAmount");
        }

        public void AttachAction(ITaskAction taskAction)
        {
            if (!CanAttachToAction(taskAction))
                throw new TaskActionException("Cannot attach this type of task action to component stack");
            if (_taskActions.Contains(taskAction))
                throw new TaskActionException("This task action is already attached");
            _taskActions.Add(taskAction);
            taskAction.Completed += OnTaskCompleted;
        }

        public void DettachAction(ITaskAction taskAction)
        {
            if (!_taskActions.Contains(taskAction))
                throw new TaskActionException("This task action is not attached");
            _taskActions.Remove(taskAction);
            taskAction.Completed -= OnTaskCompleted;
        }

        public bool CanAttachToAction(ITaskAction taskAction)
        {
            return taskAction.TaskActionType == TaskActionType.DeliveryTask ||
                   taskAction.TaskActionType == TaskActionType.PickupTask;
        }

        public void OnTaskCompleted(object sender, TaskActionEventArgs e)
        {
            DettachAction(sender as ITaskAction);
        }
    }
}