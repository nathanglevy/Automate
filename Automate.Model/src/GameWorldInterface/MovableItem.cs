using System;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;

namespace Automate.Model.GameWorldInterface
{
    /// <summary>
    /// Interface class allowing access to the Movable Model.
    /// All methods called via this interface point to a specific movable in the model.
    /// All accesses are done only via this interface as it protects the model exposing the rep
    /// and breaking the model invariants.
    /// A movable is anything that can do a MOVEMENT on the map or test for a PATH.
    /// </summary>
    public class MovableItem : Item
    {
        private readonly GameWorld _gameWorld;
        public override Coordinate Coordinate => CurrentCoordiate;

        internal MovableItem(GameWorld gameWorld, Guid movableGuid) {
            Guid = movableGuid;
            Type = ItemType.Movable;
            _gameWorld = gameWorld;
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;


            MovableItem movableItem = (MovableItem)obj;
            return (Guid == movableItem.Guid);
        }

        public override int GetHashCode() {
            return Guid.GetHashCode();
        }

        /// <summary>
        /// Property: Current coordinate of the movable
        /// </summary>
        public Coordinate CurrentCoordiate => _gameWorld.GetMovable(Guid).GetCurrentCoordinate();

        /// <summary>
        /// Property: Effective coordinate of the movable.
        /// This is used when upon starting a movement, the object is set to "start transition mode"
        /// Effective coordinate will be the next, while current will be the previous.
        /// If movable is not in transition, effective coordinate is the same as current coordinate.
        /// 
        /// Mode of use:
        /// <list>
        /// <item>Start movement animation</item>
        /// <item>Apply StartTransitionToNext() method</item>
        /// <item>===> All calls to effective coordinate here will give the NEXT coordinate instead of current</item>
        /// <item>Movment animation completes</item>
        /// <item>Apply MoveToNext() method</item>
        /// </list>
        /// </summary>
        public Coordinate EffectiveCoordinate => _gameWorld.GetMovable(Guid).GetEffectiveCoordinate();

        /// <summary>
        /// Property: Is the movable currently transitioning between two cells.
        /// This is used to check if the object is currently transitioning between two cells.
        /// Mode of use:
        /// <list>
        /// <item>Start movement animation</item>
        /// <item>Apply StartTransitionToNext() method</item>
        /// <item>===> All calls to IsTransitioning here will return true</item>
        /// <item>Movment animation completes</item>
        /// <item>Apply MoveToNext() method</item>
        /// </list>
        /// </summary>
        public bool IsTransitioning => _gameWorld.GetMovable(Guid).IsTransitioning();

        /// <summary>
        /// Property: Gives the next movement in the path.
        /// If there is no next movement, returns a movement of 0,0,0 with a cost of 0.
        /// </summary>
        public Movement NextMovement => _gameWorld.GetMovable(Guid).GetNextMovement();

        /// <summary>
        /// Property: Gives the next coordinate in the path.
        /// If there is no next coordinate, returns current coordinate.
        /// </summary>
        public Coordinate NextCoordinate => _gameWorld.GetMovable(Guid).GetNextCoordinate();

        /// <summary>
        /// Property: Movable type class. Defined in MovableType Enum.
        /// </summary>
        public MovableType MovableType => _gameWorld.GetMovable(Guid).MovableType;

        /// <summary>
        /// Property: The normalized (game speed: 1) animation speed of the movement -- how long should the movement take. 
        /// </summary>
        public double NextMovementDuration => NextMovement.GetMoveCost() / Speed;

        /// <summary>
        /// Property: The speed of this movable.
        /// </summary>
        public double Speed
        {
            get { return _gameWorld.GetMovable(Guid).Speed; }
            set { _gameWorld.GetMovable(Guid).Speed = value; }
        }

        /// <summary>
        /// Issue a movement command to a movable. It will try to plan a path to the designated target.
        /// Next coordinate will be set to be the next coordinate in the newly found path.
        /// </summary>
        /// <param name="targetCoordinate">Target coordiante. Finds a path between current position and target</param>
        /// <returns>True if a path was found and set, false if no path was found</returns>
        public bool IssueMoveCommand(Coordinate targetCoordinate)
        {
            return _gameWorld.IssueMoveCommand(Guid, targetCoordinate);
        }

        /// <summary>
        /// Check if this movable has a path set which is incompleted.
        /// This is useful to check to see if there is a next movement or path is complete.
        /// </summary>
        /// <returns>True if movable currently has a set path and it is not completed, false otherwise</returns>
        public bool IsInMotion()
        {
            return _gameWorld.GetMovable(Guid).IsInMotion();
        }

        /// <summary>
        /// Move the movable to the next cell on it's designated path. If the path does not have a next cell
        /// or there is no path, nothing happens and the movement returned is 0,0,0 with cost 0.
        /// </summary>
        /// <returns>The movement made when moving to the next cell. If no change was made returns 0,0,0 with cost 0.</returns>
        public Movement MoveToNext()
        {
            return _gameWorld.GetMovable(Guid).MoveToNext();
        }

        /// <summary>
        /// Set state to start of transitioning between two cells.
        /// Mode of use:
        /// <list>
        /// <item>Start movement animation</item>
        /// <item>Apply StartTransitionToNext() method</item>
        /// <item>===> All calls to IsTransitioning are true, and effective coordinate will refer to next coordinate</item>
        /// <item>Movment animation completes</item>
        /// <item>Apply MoveToNext() method</item>
        /// </list>
        /// </summary>
        public void StartTransitionToNext()
        {
            _gameWorld.GetMovable(Guid).StartTransitionToNext();
        }

    }
}