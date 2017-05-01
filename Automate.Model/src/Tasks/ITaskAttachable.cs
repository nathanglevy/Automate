using System;
using JetBrains.Annotations;

namespace Automate.Model.Tasks
{
    public interface ITaskAttachable
    {
        void AttachAction(TaskAction taskAction);
        void DettachAction(TaskAction taskAction);
        bool CanAttachToAction(TaskAction taskAction);
        void OnTaskCompleted(object sender, TaskActionEventArgs e);
    }
}