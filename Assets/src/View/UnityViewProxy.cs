using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers.GoAndDoSomething;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Handlers.PlaceAnObject;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Tasks;
using UnityEditor;
using UnityEngine;
using Component = Automate.Model.Components.Component;
using Object = UnityEngine.Object;

namespace src.View
{
    public class UnityViewProxy : MonoBehaviour
    {

        // Use this for initialization
        public GameViewBase GameViewBase;
        public bool MultiThreaded = true;
        public Dictionary<Guid, GameObject> _movableDictionary = new Dictionary<Guid, GameObject>();
        // Game Objects
        public GameObject CellObjectReference;
        public GameObject MovableObjectReference;
        public GameObject StructureObjectReference;
         
        Logger _logger = new Logger(new AutomateLogHandler());

        // Log Tags
        private string INPUT = "INPUT_CAPTURE";
        private string START = "START";
        private string UPDATE = "UPDATE";
        private string HANDLE_ACTION = "HANDLE_ACTION";
        public int GameXSize = 10;
        public int GameYSize = 10;

        public void Start()
        {
            _logger.Log(LogType.Log,START,"Start Method invoked");
            GameViewBase = new GameViewBase();

            
            GameViewBase.OnActionReady += HandleAction;
            var gameController = new GameController(GameViewBase) { MultiThreaded = MultiThreaded };


            // notify that Start being Done Now
            _logger.Log(LogType.Log, START, "invoke gameViewBase OnStart Events");
            GameViewBase.PerformOnStart(new Coordinate(GameXSize,GameYSize,1));

            _logger.Log(LogType.Log, START, "Finish Start Method");


        }

        public void Update()
        {
            try
            {

//                _logger.Log(LogType.Log, UPDATE, "Update Method invoked");
//                _logger.Log(LogType.Log, UPDATE, "Check If Any key pressed and handle");
                CheckForKeypressesAndAddItems();


                if (GameViewBase != null)
                {
                    GameViewBase.PerformCompleteUpdate();
                }
                else
                {
                    throw new NullReferenceException("GameViewBaseIsNull At Update Method, it's not expected");   
                }
                
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            
            }
        }


        private void HandleAction(ViewHandleActionArgs args)
        {
            _logger.Log(LogType.Log,HANDLE_ACTION,"Handle Action invoked");
            var action = args.Action;

            if (action == null)
                throw new NullReferenceException("cannot HandleAction because the recieved Action is null.");

            _logger.Log(LogType.Log, HANDLE_ACTION, "Action Type: " + action.Type);
            switch (action.Type)
            {
                case ActionType.AreaSelection:
                    break;
                case ActionType.Movement:
                    
                    MoveObject(action as MoveAction);
                    break;
                case ActionType.SelectPlayer:
                    break;
                case ActionType.PlaceGameObject:
                    PlaceAnObject(action as PlaceAGameObjectAction);
                    break;
                case ActionType.Deliver:
                    var deliverAction = action as DeliverAction;
                    SetStructureAtCoordinate(deliverAction.TargetDest, "SpriteSheets/open_tileset_2x", 547, StructureObjectReference, 1);
                    _logger.Log(string.Format("Deliver Is Executed of Amount: {0} AT Location: {1}", deliverAction.Amount, deliverAction.TargetDest));
                    //Object.Instantiate(CellObjectReference,
                    //    GetWorldVectorFromMapCoodinates(deliverAction.TargetDest), Quaternion.identity);

                    break;
                case ActionType.PickUp:
                    var pickUpAction = action as PickUpAction;
                    SetStructureAtCoordinate(pickUpAction.TargetDest, "SpriteSheets/open_tileset_2x", 546, CellObjectReference, 1 );
                    _logger.Log(string.Format("PickUp Is Executed of Amount: {0} AT Location: {1}", pickUpAction.Amount, pickUpAction.TargetDest));
                    //Object.Instantiate(CellObjectReference,
                    //    GetWorldVectorFromMapCoodinates(pickUpAction.TargetDest), Quaternion.identity);

                    //EditorUtility.DisplayDialog("PickUp Action",string.Format("PickUp Is Executed of Amount: {0} AT Locatin: {1}", pickUpAction.Amount, pickUpAction.TargetDest
                    //), "Yes, Great ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveObject(MoveAction moveAction)
        {
            _logger.Log(LogType.Log, HANDLE_ACTION, String.Format("MOVE OBJECT FROM: {0} to {1}", moveAction.CurrentCoordiate, moveAction.To));
            Vector3 movableStartVector = GetWorldVectorFromMapCoodinates(moveAction.CurrentCoordiate);
            Vector3 movableTargetVector = GetWorldVectorFromMapCoodinates(moveAction.To);
            GameObject movableGameObject = _movableDictionary[moveAction.TargetId];

            movableGameObject.GetComponent<MovableBehaviour>().startPosition = movableStartVector;
            movableGameObject.GetComponent<MovableBehaviour>().targetPosition = movableTargetVector;
            movableGameObject.GetComponent<MovableBehaviour>().animationSpeed = (float) moveAction.Duration.TotalSeconds ;
            movableGameObject.GetComponent<MovableBehaviour>().isMoving = true;
            movableGameObject.GetComponent<MovableBehaviour>().journeyFract = 0;
            string animationName = movableGameObject.GetComponent<MovableBehaviour>().DecideAnimation();
            movableGameObject.GetComponent<Animator>().Play(animationName);

            _logger.Log(LogType.Log, HANDLE_ACTION, String.Format("Animate Selected: {0} for ID: {1}", animationName,moveAction.TargetId));}

        private void PlaceAnObject(PlaceAGameObjectAction action)
        {
            if (action == null)
                throw new ArgumentException("got a null instead of PlaceAGameObjectAction");

            _logger.Log(LogType.Log, HANDLE_ACTION, "Place an Object: " + action.ItemType);

            switch (action.ItemType)
            {
                case ItemType.Movable:
                    _logger.Log(LogType.Log, HANDLE_ACTION, String.Format("Adding Movable {0} at {1}.", "Human", action.Coordinate));
                    GameObject newGameObject = Object.Instantiate(MovableObjectReference,
                    GetWorldVectorFromMapCoodinates(action.Coordinate)  + Vector3.back * 2, Quaternion.identity);
                    _movableDictionary.Add(action.Id,newGameObject);
                    break;
                case ItemType.Structure:
                    _logger.Log(LogType.Log, HANDLE_ACTION, String.Format("Adding Structure at {0}.", action.Coordinate));
                    var spritesheetsOpenTilesetX = "SpriteSheets/open_tileset_2x";
                    var spriteNum = 641;
                    SetStructureAtCoordinate(action.Coordinate, spritesheetsOpenTilesetX, spriteNum, StructureObjectReference, 3);
                    break;
                case ItemType.Cell:
                     Object.Instantiate(CellObjectReference,
                GetWorldVectorFromMapCoodinates(action.Coordinate) , Quaternion.identity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            



        }

        private void SetStructureAtCoordinate(Coordinate cor, string spritePath, int SprintNum, GameObject targetGameObject, float z)
        {
            GameObject structureObject = Object.Instantiate(targetGameObject,
                GetWorldVectorFromMapCoodinates(cor) + Vector3.back * z, Quaternion.identity);
            GraphicsHandler.SetSpriteByPath(structureObject, spritePath, SprintNum);
        }

        Coordinate GetMapCoordinateFromWorldVector(Vector3 worldVector)
        {
            Vector3 spriteSize = GetSpriteRealSize(CellObjectReference);
            return new Coordinate((int)(worldVector.x / spriteSize.x), (int)(worldVector.y / spriteSize.y), 0);
        }

        Vector3 GetMouseEffectiveWorldLocation()
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition + GetSpriteRealSize(CellObjectReference) / 2;
        }

        void CheckForKeypressesAndAddItems()
        {
            Vector3 worldPosition = GetMouseEffectiveWorldLocation();
            Coordinate mapCoordinate = GetMapCoordinateFromWorldVector(worldPosition);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _logger.Log(LogType.Log, INPUT,
                    "Key: 1 preseed, we will place Structure at " + mapCoordinate.ToString());
                var placeAMovableRequest =
                    new PlaceAStrcutureRequest(mapCoordinate, new Coordinate(1, 1, 1), StructureType.Basic);
                GameViewBase.Controller.Handle(placeAMovableRequest);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _logger.Log(LogType.Log, INPUT, "Key: 2 preseed, we will place movable at " + mapCoordinate.ToString());
                Debug.Log("Key: 2 preseed, we will place movable at " + mapCoordinate.ToString());
                var placeAMovableRequest = new PlaceAMovableRequest(mapCoordinate, MovableType.NormalHuman);
                GameViewBase.Controller.Handle(placeAMovableRequest);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _logger.Log(LogType.Log, INPUT, "Key: 3 preseed, we will clear selected items");
                var gameWorldItemById = GameUniverse.GetGameWorldItemById(GameViewBase.Controller.Model);
                gameWorldItemById.ClearSelectedItems();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetStructureAtCoordinate(new Coordinate(0, 9, 0), "SpriteSheets/open_tileset_2x", 548,
                    CellObjectReference, 1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {


                var gameWorld = GameUniverse.GetGameWorldItemById(GameViewBase.Controller.Model);

                // create Pickup Component Stack
                var cmpntGrp330 = gameWorld.GetComponentStackGroupAtCoordinate(new Coordinate(0, 9, 0));
                var ironat330 = cmpntGrp330.AddComponentStack(ComponentType.IronOre, 50);

                // create Delivery Component Stack
                var cmpntGrp000 = gameWorld.GetComponentStackGroupAtCoordinate(new Coordinate(0, 0, 0));
                var ironat000 = cmpntGrp000.AddComponentStack(ComponentType.IronOre, 0);

                // create Delivery Component Stack
                var cmpntGrp220 = gameWorld.GetComponentStackGroupAtCoordinate(new Coordinate(9, 0, 0));
                var ironat220 = cmpntGrp220.AddComponentStack(ComponentType.IronOre, 0);



                // Create the Task and Actions
                var PickupAndDeliverTask = gameWorld.TaskDelegator.CreateNewTask();
                PickupAndDeliverTask.AddTransportAction(TaskActionType.PickupTask, new Coordinate(0, 9, 0), cmpntGrp330,
                    Component.IronOre, 40);
                PickupAndDeliverTask.AddTransportAction(TaskActionType.DeliveryTask, new Coordinate(0, 0, 0), cmpntGrp000,
                    Component.IronOre, 30);
                PickupAndDeliverTask.AddTransportAction(TaskActionType.DeliveryTask, new Coordinate(9, 0, 0), cmpntGrp000,
                    Component.IronOre, 5);

                var movableListInCoordinate = gameWorld.GetMovableListInBoundary(new Boundary(new Coordinate(0, 0, 0), new Coordinate(GameXSize, GameYSize, 0)));
                

                var movementPathWithLowestCostToCoordinate = gameWorld.GetMovementPathWithLowestCostToCoordinate(
                    movableListInCoordinate.Select(p => p.CurrentCoordiate).ToList(), new Coordinate(0, 9, 0));
                var targetMovable = movableListInCoordinate.First(p => p.CurrentCoordiate.Equals(movementPathWithLowestCostToCoordinate.GetStartCoordinate()));
                gameWorld.TaskDelegator.AssignTask(targetMovable.Guid, PickupAndDeliverTask);
                gameWorld.SelectMovableItems(new List<MovableItem>() { targetMovable });

                
                GameViewBase.Controller.Handle(new TaskContainer(PickupAndDeliverTask) { OnCompleteDelegate = TaskFinished });

            }


            if (Input.GetMouseButtonDown(0))
            {
                _logger.Log(LogType.Log, INPUT, "Mouse Key: 0(LEFT) preseed, we will select any items at " + mapCoordinate.ToString());
                    var viewSelectionNotification = new ViewSelectionNotification(mapCoordinate, mapCoordinate, Guid.NewGuid());
                GameViewBase.Controller.Handle(viewSelectionNotification);
            }
            if (Input.GetMouseButtonDown(1))
            {
                _logger.Log(LogType.Log, INPUT, "Mouse Key: 1(RIGHT) preseed, we will move any selected objects to " + mapCoordinate.ToString());
                var rightSelectNotification = new RightClickNotification(mapCoordinate);
                GameViewBase.Controller.Handle(rightSelectNotification);
            }
            if (Input.GetMouseButtonDown(2))
            {
                //_logger.Log(LogType.Log, INPUT, "Mouse Key: 2(MIDDLE) preseed, we will FORCE move selected objects to " + mapCoordinate.ToString());
                //var rightSelectNotification = new RightClickNotification(mapCoordinate) { Force = true };
                //GameViewBase.Controller.Handle(rightSelectNotification);
            }
        }

        private void TaskFinished(ControllerNotificationArgs args)
        {
            _logger.Log(LogType.Log, INPUT, "Distrbuting 40 Iron at 2 points was done");
            GameViewBase.Controller.Handle(new RightClickNotification(new Coordinate(5,5,0)));

        }


        Vector3 GetWorldVectorFromMapCoodinates(Coordinate coordinate)
        {
            Vector3 spriteSize = GetSpriteRealSize(CellObjectReference);
            return new Vector3(coordinate.x * spriteSize.x, coordinate.y * spriteSize.y, 0);
        }

        Vector3 GetSpriteRealSize(GameObject gameObject)
        {
            Vector2 spriteSize = gameObject.GetComponent<SpriteRenderer>().sprite.rect.size;
            Vector2 localSpriteSize = spriteSize / gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
            return localSpriteSize;
        }




 

    }
}

