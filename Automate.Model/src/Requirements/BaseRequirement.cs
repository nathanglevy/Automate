using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements
{
    public abstract class BaseRequirement
    {
        public int Amount = 0;
        public bool IsSatisfied => RequirementRemainingToSatisfy <= 0;
        public bool IsDelegated => RequirementRemainingToDelegate <= 0;
        public int RequirementRemainingToSatisfy => Amount;
        public int RequirementRemainingToDelegate => RequirementRemainingToSatisfy - _taskActions.Sum(item => item.Amount);
        public int TotalRequirement { get; internal set; }
        protected readonly List<ITaskAction> _taskActions = new List<ITaskAction>();
        public Guid Guid { get; } = Guid.NewGuid();
    }
}