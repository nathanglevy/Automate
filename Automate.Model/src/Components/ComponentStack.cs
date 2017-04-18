using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Components
{
    public class ComponentStack
    {
        public Guid Guid { get; }
        public Component ComponentType { get; }
        public int Amount { get; private set; }
        public int UnallocatedAmount => Amount - OutgoingAllocatedAmount;
        public int OutgoingAllocatedAmount => _outgoingAllocations.Values.Sum();
        public int IncomingAllocatedAmount => _incomingAllocations.Values.Sum();

        private Dictionary<Guid,int> _outgoingAllocations = new Dictionary<Guid, int>();
        private Dictionary<Guid,int> _incomingAllocations = new Dictionary<Guid, int>();

        public ComponentStack(Component componentType, int amount)
        {
            CheckPositiveValue(amount);
            Guid = Guid.NewGuid();
            ComponentType = componentType;
            Amount = amount;
        }

        public int AddAmount(int amount)
        {
            CheckPositiveValue(amount);
            return Amount += amount;
        }

        public int RemoveAmount(int amount)
        {
            CheckPositiveValue(amount);
            if (amount > Amount)
                throw new ArgumentOutOfRangeException(nameof(amount),"Cannot remove the following amount: " + amount + ", amount is higher than existing amount: " + Amount);

            return Amount -= amount;
        }

        public void AssignOutgoingAmount(Guid targetGuid, int amount)
        {
            CheckPositiveValue(amount);
            if (UnallocatedAmount < amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot allocate outgoing more than existing amount");

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
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot unassign more than assigned amount");

            if (_outgoingAllocations[targetGuid] == amount)
                _outgoingAllocations.Remove(targetGuid);
            else
                _outgoingAllocations[targetGuid] -= amount;
        }

        public void AssignIncomingAmount(Guid targetGuid, int amount)
        {
            CheckPositiveValue(amount);
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
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot unassign more than assigned amount");

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
                throw new ArgumentException("Method does not accept a negative amount");
        }
    }
}