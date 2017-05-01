using System;

namespace Automate.Model.Tasks
{
    public delegate void CompletedEventHandler(object sender, TaskActionEventArgs e);
    public class TaskActionEventArgs : EventArgs
    {
        public int Amount { get; set; } = 0;

        public TaskActionEventArgs(int amount)
        {
            Amount = amount;
        }
    }
}