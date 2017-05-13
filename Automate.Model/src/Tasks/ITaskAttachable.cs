using System;
using JetBrains.Annotations;

namespace Automate.Model.Tasks
{
    public interface ITaskAttachable
    {
        void AttachAction(ITaskAction taskAction);
        void DettachAction(ITaskAction taskAction);
        bool CanAttachToAction(ITaskAction taskAction);
        void OnTaskCompleted(object sender, TaskActionEventArgs e);
        Guid Guid { get; }
    }
}