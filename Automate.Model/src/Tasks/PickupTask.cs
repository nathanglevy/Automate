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
    }
}