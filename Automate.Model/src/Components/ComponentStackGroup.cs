using System;
using System.Collections.Generic;
using System.Linq;

namespace Automate.Model.Components
{
    public class ComponentStackGroup
    {
        public const int DEFAULT_STACK_MAX_SIZE = 1000;
        public const int DEFAULT_STACKS_COUNT = 1;
        private Dictionary<string, ComponentStack> _componentStacks = new Dictionary<string, ComponentStack>();
        public float CurrentTotalWeight => _componentStacks.Sum(pair => pair.Value.CurrentTotalWeight);
        public float CurrentTotalSpace => _componentStacks.Sum(pair => pair.Value.CurrentTotalSpace);
        public float TotalIncomingWeight => _componentStacks.Sum(pair => pair.Value.TotalIncomingWeight);
        public float TotalIncomingSpace => _componentStacks.Sum(pair => pair.Value.TotalIncomingSpace);
        public int MaxSize { get; set; } = DEFAULT_STACK_MAX_SIZE;
        public int MaxStacks { get; set; } = DEFAULT_STACKS_COUNT;

        internal ComponentStackGroup(int maxSize, int maxStacks)
        {
            MaxSize = maxSize;
            MaxStacks = maxStacks;
        }

        internal ComponentStackGroup()
        {
            MaxSize = DEFAULT_STACK_MAX_SIZE;
            MaxStacks = DEFAULT_STACKS_COUNT;
        }

        public void AddComponentStack(Component component, int amount)
        {
            ComponentStack newStack = new ComponentStack(component, amount);
            if (CurrentTotalSpace + newStack.CurrentTotalSpace > MaxSize)
                throw new ArgumentOutOfRangeException("Not enough space in stackgroup to add: " + amount + " of " + component.Type);
            _componentStacks.Add(newStack.ComponentType.Type, newStack);
        }

        public ComponentStack GetComponentStack(Component component) {
            if (!_componentStacks.ContainsKey(component.Type))
                throw new ArgumentException("component does not exist in group: " + component.Type);
            return _componentStacks[component.Type];
        }

        public ComponentStack GetComponentStack(ComponentType componentType) {
            if (!_componentStacks.ContainsKey(componentType.ToString()))
                throw new ArgumentException("component does not exist in group: " + componentType.ToString());
            return _componentStacks[componentType.ToString()];
        }

        public void RemoveComponentStack(Component component)
        {
            if (!_componentStacks.ContainsKey(component.Type))
                throw new ArgumentException("component does not exist in group: " + component.Type);
            _componentStacks.Remove(component.Type);
        }

        public List<string> GetListOfComponentsInGroup()
        {
            return _componentStacks.Select(pair => pair.Key).ToList();
        }

        public List<ComponentStack> GetListOfComponentStacksInGroup() {
            return _componentStacks.Select(pair => pair.Value).ToList();
        }

        public void TransferToStackGroup(Guid idOfTransferer, ComponentStackGroup componentStackGroup, Component component, int amount)
        {
            //TODO add checks and only commit transfer if it is successful!!!
            _componentStacks[component.Type].PickupAmount(idOfTransferer, amount);
            componentStackGroup.GetComponentStack(component).DeliverAmount(idOfTransferer, amount);
        }

        public void TransferToStack(Guid idOfTransferer, ComponentStack componentStack, int amount) {
            //TODO add checks and only commit transfer if it is successful!!!
            _componentStacks[componentStack.ComponentType.Type].PickupAmount(idOfTransferer, amount);
            componentStack.DeliverAmount(idOfTransferer, amount);
        }
    }
}