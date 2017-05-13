using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Requirements;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;
using UnityEngine.Events;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class RequirementsHandler : Handler<IObserverArgs>
    {
        private Dictionary<Guid, int> numberOfWorkersPerGuid = new Dictionary<Guid, int>();

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            try
            {


                if (!CanHandle(args))
                    throw new ArgumentException(
                        "only Args from the type RequirementsPackage can be passed to Handle Method");

                // Cast and Get Requirement
                var reqWrapper = args as RequirementsPackage;
                var req = reqWrapper.Requirement;

                // Get GameWorld
                var gameWorld = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
                var structuresWithJobInProgress = gameWorld.RequirementAgent.GetStructuresWithJobInProgress();

                // iterate over all structure active jobs
                var tasks = new List<MasterAction>();
                foreach (var structure in structuresWithJobInProgress)
                {
                    // Create a HashSet which will hold Requirments which blocked to any reason
                    var blockedReq = new HashSet<Guid>();

                    // Loop till getting
                    while (structure.HasJobInProgress && GetNumberOfWorkers(structure) <
                           GetMaxNumberOfWorkers(structure))
                    {
                        var nonBlockedInCompleteReqs = structure.CurrentJob.JobRequirements
                            .GetIncompleteTransportRequirements()
                            .Where(p => !blockedReq.Contains(p.Guid));
                        if (!nonBlockedInCompleteReqs.Any())
                            break; // Break the While Loop, we cannot continue all Requriments are blocked

                        foreach (var requirement in nonBlockedInCompleteReqs)
                        {

                            MasterAction action = null;
                            switch (requirement.RequirementType)
                            {

                                case RequirementType.ComponentPickup:
                                    action = ExecutePickupJob(structure, requirement, gameWorld);
                                    break;
                                case RequirementType.ComponentDelivery:
                                    action = ExecuteDeliveryJob(structure, requirement, gameWorld);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(String.Format(
                                        (string) "Requirement Type:{0} not Supported",
                                        (object) requirement.RequirementType));
                            }

                            // Check if we got anything from the handler, if not, assign requirments as blocked
                            if (action == null)
                                blockedReq.Add(requirement.Guid);
                            else
                            {
                                tasks.Add(action);
                                IncWorkersListForItem(structure);
                                if (requirement.RequirementRemainingToDelegate == 0)
                                    blockedReq.Add(requirement.Guid);
                            }
                        }
                    }
                }


                // Reset the Dict to Start from Clean At Next Time
                numberOfWorkersPerGuid.Clear();

                return new HandlerResult(tasks) {IsInternal = true};
            }
            catch (Exception e)
            {
                throw new Exception("a problem was occurred when trying to handle the requirment: "+ e.ToString());
            }
        }

        private MasterAction ExecuteDeliveryJob(IStructure structure, ITransportRequirement requirement, IGameWorld gameWorld)
        {

            // Iterate over all techinques to Get Delivery 
            // 1. find idle movable with component
            // 2. find "in transition" movable targets Storage
            // 3. idle movable to closet Pickup Location If Any To Delivery
            // 4. idle movable to storage point then To Delivery

            List<ITransportScenarioProvider> scenarios = new List<ITransportScenarioProvider>()
            {
                new CarriedMovableDirectRoute(),

            };

            SortedList<float,DeliveryCost> costs = new SortedList<float,DeliveryCost>();
            foreach (var deliveryProvider in scenarios)
            {
                var scenarioCost = deliveryProvider.CalcScenarioCost(structure.CurrentJob, requirement, structure.Boundary,
                    gameWorld);
                costs.Add(scenarioCost.Cost,scenarioCost);
            }

            // Now we have all scenarios and associated cost, let's pick the minumum and execute
            var minCost = costs.Min();

            if (minCost.Key == float.PositiveInfinity)
                return null;

            // Allocate InComing to Target Component
            Guid executingMovable = minCost.Value.ScenarioTask.TargetTask.AssignedToGuid;
            structure.ComponentStackGroup.GetComponentStack(requirement.Component).AssignIncomingAmount(executingMovable,minCost.Value.Amount );
            gameWorld.GetMovable(executingMovable).ComponentStackGroup.GetComponentStack(requirement.Component).AssignOutgoingAmount(executingMovable,minCost.Value.Amount);


            // Add/Assign/Commit new Task to TaskDelegator
            gameWorld.TaskDelegator.AddAndCommitNewTask(minCost.Value.ScenarioTask.TargetTask);

            // return to start handling

            // return the TaskContainer associated with Minumum Cost
            return minCost.Value.ScenarioTask;
        }

        private void IncWorkersListForItem(IStructure structureJobReq)
        {
            if (!numberOfWorkersPerGuid.ContainsKey(structureJobReq.Guid))
            {
                numberOfWorkersPerGuid.Add(structureJobReq.Guid, 0);
            }
            numberOfWorkersPerGuid[structureJobReq.Guid]++;
        }

        private int GetMaxNumberOfWorkers(IStructure structureJobReq)
        {
            // TODO: move this according to the structure when support will be added
            return 2;
        }

        private int GetNumberOfWorkers(IStructure structureJobReq)
        {
            if (!numberOfWorkersPerGuid.ContainsKey(structureJobReq.Guid))
                return 0;
            // TODO: move this to structure when support will be added
            return numberOfWorkersPerGuid[structureJobReq.Guid];
        }

        private MasterAction ExecutePickupJob(IStructure structure, ITransportRequirement requirement,
            IGameWorld gameWorld)
        {
      
            // Create a Dict to hold cost per Movable seeking for best/cheapest cost to do the job
            var costPerMovableDict = new SortedDictionary<float, List<IMovable>>();
            var targetComponent = requirement.Component;

            // iterate over all idle movables
            foreach (var movable in gameWorld.GetMovableList().Where(m => !m.IsInMotion() &&
            m.ComponentStackGroup.GetComponentStack(targetComponent).RemainingAmountForIncoming > 0))
            {
                // Check if we already assigned this Movable to other Req/Task at this Phase
                if (gameWorld.TaskDelegator.HasDelegatedTasks(movable.Guid)) 
                    continue;

                // check if movable Can Do the Job according to it's Skils
                if (!CanMovableDoTheJob(movable, structure.CurrentJob))
                    continue;

                // Calculate the Cost doing the requirment using this movable
                var cost = CalculateJobCost(structure, movable, gameWorld);

                // push it to the dict
                if (!costPerMovableDict.ContainsKey(cost))
                {
                    costPerMovableDict.Add(cost,new List<IMovable>());
                }
                costPerMovableDict[cost].Add(movable);
            }

            if (costPerMovableDict.Count == 0)
                return null;    // No Movable Found

            // now we will pick the cheaper and execute the Task
            var matchMovableKey = costPerMovableDict.Min(p => p.Key);

            // Get Component Type and Find the Max Amount of Component can be delivered


            var matchMovable = costPerMovableDict[matchMovableKey].Aggregate(
                (bestMatch,current)=> 
                bestMatch.ComponentStackGroup.GetComponentStack(targetComponent).RemainingAmountForIncoming > current.ComponentStackGroup.GetComponentStack(targetComponent).RemainingAmountForIncoming ? bestMatch : current);


            var amount = GetMin(new List<int>()
            {
                matchMovable.ComponentStackGroup.GetComponentStack(targetComponent).RemainingAmountForIncoming, // Whatever the movable can "carry"
                requirement.TotalRequirement, 
                requirement.RequirementRemainingToDelegate,
                requirement.RequirementRemainingToSatisfy,
            });

            if (amount == 0)
                return null;

            // Create a Task 
            var newTask = new Task();
            
            // Add/Assign/Commit new Task to TaskDelegator
            gameWorld.TaskDelegator.AddAndCommitNewTask(newTask);

            // Assign Task To Best Match Movable
            //gameWorld.TaskDelegator.AssignTask(matchMovable.Guid, newTask);
            newTask.AssignedToGuid = matchMovable.Guid;

            // Update Outgoing Amount At Pickup 
            var stackGroup = structure.ComponentStackGroup;
            var targetStack = stackGroup.GetComponentStack(targetComponent);
            targetStack.AssignOutgoingAmount(matchMovable.Guid,amount);

            // Update Incoming Amount At Movable
            var movableTargetStack = matchMovable.ComponentStackGroup.GetComponentStack(targetComponent);
            movableTargetStack.AssignIncomingAmount(matchMovable.Guid, amount);

            // Create the Transport Pickup Action in Task
            var transportAction = newTask.AddTransportAction(TaskActionType.PickupTask, structure.Coordinate,stackGroup
                , targetComponent, amount);

            // Attach action to the requirment
            requirement.AttachAction(transportAction);
            
            // return to start handling
            return new TaskContainer(newTask);
        }

        private int GetMin(List<int> nubmers)
        {
            return nubmers.Min();
        }

        private float CalculateJobCost(IStructure structure, IMovable movable, IGameWorld gameWorld)
        {
            // TODO: Consult Naph on this, Cost should be a number which contains/reflects:
            // 1. path to the Dest
            // 2. Speed of Movable
            // 3. CarryCapability
            // ?

            // for now i will return number of Steps to Target as COST
            var pathToDest = gameWorld.GetMovementPathWithLowestCostToBoundary(
                new List<Coordinate>() {movable.CurrentCoordinate},
                structure.Boundary,false);


            return pathToDest.TotalCost;
        }

        private bool CanMovableDoTheJob(IMovable movable, RequirementJob currentJob)
        {
            // TODO: enhance the code when Movable Skills will be implemented
            return true;
        }

        //    var tasks = new List<MasterAction>();
        //    switch (req.RequirementType)
        //    {
        //        case RequirementType.ComponentDelivery:
        //            break;
        //        case RequirementType.ComponentPickup:
        //            TaskContainer task = CreatePickUpTask(reqWrapper,gameWorld);
        //            tasks.Add(task);
        //            break;
        //        case RequirementType.Environment:
        //            break;
        //        case RequirementType.Work:
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }

        //    return new HandlerResult(tasks);
        //}

        //private TaskContainer CreatePickUpTask(RequirementsPackage req, IGameWorld gameWorld)
        //{
        //    // create new Task
        //    var pickupTask = gameWorld.TaskDelegator.CreateNewTask();

        //    // Get/Add Component Stack Group at Dest
        //    var cmpntGrp = gameWorld.GetComponentStackGroupAtCoordinate(req.HostingItem.Coordinate);

        //    // Create Transport Action
        //    var pickUpTaskAction = pickupTask.AddTransportAction(TaskActionType.PickupTask, req.HostingItem.Coordinate, cmpntGrp,
        //        req.Requirement.Component, req.Requirement.Amount);

        //    // Link the Req to the Task
        //    req.Requirement.AttachAction(pickUpTaskAction);

        //    if (IsIdleMovablesExist(gameWorld))
        //    {
        //        gameWorld.TaskDelegator.AssignTask(FindClosetMovable(gameWorld,req.HostingItem.Coordinate),pickupTask);
        //    }
        //    return new TaskContainer(pickupTask);
        //}

        private Guid FindClosetMovable(IGameWorld gameWorld, Coordinate targetDest)
        {
            var idleMovables = gameWorld.GetMovableIdList()
                .Where(p => !gameWorld.GetMovable(p).IsInMotion())
                .Select(gameWorld.GetMovable)
                .ToList();

            var closestPath =
                gameWorld.GetMovementPathWithLowestCostToCoordinate(idleMovables.Select(p => p.Coordinate).ToList(),
                    targetDest);
            var targteMovablemovable = idleMovables.First(p => p.Coordinate.Equals(closestPath.GetStartCoordinate()));
            return targteMovablemovable.Guid;
        }

        private bool IsIdleMovablesExist(IGameWorld gameWorld)
        {
            List<Coordinate> idleMovables = new List<Coordinate>();
            foreach (var movableId in gameWorld.GetMovableIdList())
            {
                if (!gameWorld.GetMovable(movableId).IsInMotion())
                    return true;
            }
            return false;
        }


        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("Args is null, cannot determine if Handler should be activated, dying");
            return args is RequirementsPackage;
        }
    }
}