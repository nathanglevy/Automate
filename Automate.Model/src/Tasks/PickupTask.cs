using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks
{
    public class PickupTask : Task
    {
        public PickupTask(Coordinate targetCoordinate, int targetAmount)
        {
            TargetCoordinate = targetCoordinate;
            TargetAmount = targetAmount;
        }

        public Coordinate TargetCoordinate { get; }
        public int TargetAmount { get; private set; }

        public PickupTask SplitPickupTask(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Cannot use negative number to split tasks");
            if (TargetAmount <= amount)
                throw new ArgumentException("Cannot split pickup task because split amount: " + amount + " is larger than source task amount " + TargetAmount);

            PickupTask pickupTask = new PickupTask(TargetCoordinate, amount);
            TargetAmount -= amount;
            return pickupTask;
        }

        public void MergeWithPickupTask(PickupTask pickupTaskToMerge)
        {
            if (!pickupTaskToMerge.TargetCoordinate.Equals(TargetCoordinate))
                throw new ArgumentException("Cannot merge pickup tasks not in same coordinate!");
            TargetAmount += pickupTaskToMerge.TargetAmount;
        }

        public static PickupTask MergePickupTasks(PickupTask task1, PickupTask task2) {
            if (!task1.TargetCoordinate.Equals(task2.TargetCoordinate))
                throw new ArgumentException("Cannot merge pickup tasks not in same coordinate!");
            return new PickupTask(task1.TargetCoordinate, task1.TargetAmount + task2.TargetAmount);
        }

        public override bool IsPositionChangeRequired(Coordinate currentPosition)
        {
            return (!TargetCoordinate.Equals(currentPosition));
        }
    }
}