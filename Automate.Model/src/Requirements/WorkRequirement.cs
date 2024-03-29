﻿using System;
using System.Collections.Generic;
using Automate.Model.Components;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements
{
    public class WorkRequirement : IRequirement
    {
        public bool IsSatisfied => RequirementRemainingToSatisfy <= 0;
        public RequirementType RequirementType { get; } = RequirementType.Work;
        public int RequirementRemainingToSatisfy { get; private set; }
        public int TotalRequirement { get; }
        public Component ConsumedComponentType { get; }

        public Guid Guid { get; } = Guid.NewGuid();
        //TODO:
        public int RequirementRemainingToDelegate { get; }

        private List<ITaskAction> _attachedTasks = new List<ITaskAction>();

        public WorkRequirement(Component consumesComponent, int amount)
        {
            RequirementRemainingToSatisfy = amount;
            TotalRequirement = amount;
            ConsumedComponentType = consumesComponent;
        }

        public bool SatisfyRequirement(int amount)
        {
            if (amount > RequirementRemainingToSatisfy)
                throw new RequirementException("Cannot satisfy more than requirement!");
            RequirementRemainingToSatisfy -= amount;
            return IsSatisfied;
        }

        public void AttachAction(ITaskAction taskAction)
        {
            throw new System.NotImplementedException();
        }

        public void DettachAction(ITaskAction taskAction)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAttachToAction(ITaskAction taskAction)
        {
            throw new System.NotImplementedException();
        }

        public void OnTaskCompleted(object sender, TaskActionEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        
    }
}