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
            if (!CanHandle(args))
                throw new ArgumentException(
                    "only Args from the type RequirementsPackage can be passed to Handle Method");

            // Cast and Get Requirement
            var reqWrapper = args as RequirementsPackage;
            var req = reqWrapper.Requirement;

            // Get GameWorld
            var gameWorld = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
            var agent = gameWorld.RequirementAgent;
            var structuresJobs = gameWorld.GetStructuresList().Where(p => p.HasActiveJob);

            // iterate over all structure active jobs
            var tasks = new List<MasterAction>();
            foreach (var structureJobReq in structuresJobs)
            {
                //TODO: check if HasInProgress is better
                while (structureJobReq.HasActiveJob && GetNumberOfWorkers(structureJobReq) <
                       GetMaxNumberOfWorkers(structureJobReq))
                {
                    foreach (var requirement in structureJobReq.CurrentJob.JobRequirements.GetIncompleteRequirements())
                    {
                        switch (requirement.RequirementType)
                        {
                            case RequirementType.ComponentPickup:
                                var executePickupJob = ExecutePickupJob(structureJobReq, requirement, gameWorld);
                                tasks.Add(executePickupJob);
                                break;
                            case RequirementType.ComponentDelivery:
                                var executeDeliveryJob = ExecuteDeliveryJob(structureJobReq, requirement, gameWorld);
                                tasks.Add(executeDeliveryJob);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(String.Format((string) "Requirement Type:{0} not Supported", (object) requirement.RequirementType));
                        }
                    }
                }
            }

            return new HandlerResult(tasks) {IsInternal = true};
        }

        private MasterAction ExecuteDeliveryJob(IStructure structureJobReq, IRequirement requirement, IGameWorld gameWorld)
        {
            // Create a Task 
            var newTask = gameWorld.TaskDelegator.CreateNewTask();

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
                var scenarioCost = deliveryProvider.CalcScenarioCost(structureJobReq.CurrentJob, requirement, structureJobReq.Coordinate,
                    gameWorld);
                costs.Add(scenarioCost.Cost,scenarioCost);
            }

            // Now we have all scenarios and associated cost, let's pick the minumum and execute
            var minCost = costs.Min();

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

        private MasterAction ExecutePickupJob(IStructure structureJobReq, IRequirement requirement,
            IGameWorld gameWorld)
        {
            // Create a Task 
            var newTask = gameWorld.TaskDelegator.CreateNewTask();
            

            // Create a Dict to hold cost per Movable seeking for best/cheapest cost to do the job
            var costPerMovableDict = new SortedDictionary<float, IMovable>();

            // iterate over all idle movables
            foreach (var movable in gameWorld.GetMovableList().Where(m => !m.IsInMotion()))
            {
                // Check if we already assigned this Movable to other Req/Task at this Phase
                if (HasMovableAlreadyAssigned(movable.Guid, gameWorld))
                    continue;

                // check if movable Can Do the Job according to it's Skils
                if (!CanMovableDoTheJob(movable, structureJobReq.CurrentJob))
                    continue;

                // Calculate the Cost doing the requirment using this movable
                var cost = CalculateJobCost(structureJobReq, movable, gameWorld);

                // push it to the dict
                costPerMovableDict.Add(cost, movable);
            }
            // now we will pick the cheaper and execute the Task
            var matchMovableKey = costPerMovableDict.Min(p => p.Key);
            var matchMovable = costPerMovableDict[matchMovableKey];

            // Add Worker to GUID
            IncWorkersListForItem(structureJobReq);
            gameWorld.TaskDelegator.AssignTask(matchMovable.Guid, newTask);


            var targetComponent = Component.IronOre;
            var amount = GetMin(new List<int>()
            {
                matchMovable.ComponentStackGroup.GetComponentStack(targetComponent).StackMax,
                requirement.TotalRequirement,
                requirement.RequirementRemainingToSatisfy
            });

            var transportAction = newTask.AddTransportAction(TaskActionType.PickupTask, structureJobReq.Coordinate,
                gameWorld.GetComponentStackGroupAtCoordinate(structureJobReq.Coordinate), targetComponent, amount);
            requirement.AttachAction(transportAction);

            return new TaskContainer(newTask);
        }

        private bool HasMovableAlreadyAssigned(Guid movableGuid, IGameWorld gameWorld)
        {
            try
            {
                if (gameWorld.TaskDelegator.HasDelegatedTasks(movableGuid));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
            var pathToDest = gameWorld.GetMovementPathWithLowestCostToCoordinate(
                new List<Coordinate>() {movable.CurrentCoordinate},
                new Coordinate(structure.Coordinate.x - 1, structure.Coordinate.y, 0));


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