using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers.PlaceAnObject;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace src.View
{
    public class UnityViewProxy : MonoBehaviour
    {

        // Use this for initialization
        public GameViewBase GameViewBase;
        public bool IsGameBaseNull = false;
        public Dictionary<Guid, GameObject> _movableDictionary = new Dictionary<Guid, GameObject>();
        // Game Objects
        public GameObject CellObjectReference;
        public GameObject MovableObjectReference;
        public GameObject StructureObjectReference;


        public void Start()
        {

            GameViewBase = new GameViewBase();
            GameViewBase.OnActionReady += HandleAction;
            var gameController = new GameController(GameViewBase);

            IsGameBaseNull = GameViewBase == null;

            // notify that Start being Done Now
            GameViewBase.PerformOnStart();

        }

        public void Update()
        {
            try
            {
                IsGameBaseNull = GameViewBase == null;

                CheckForKeypressesAndAddItems();

                if (IsGameBaseNull)
                {
                    Debug.LogError("GAME BASE IS NULL");
                }
                else
                {
                    GameViewBase.PerformCompleteUpdate();
                }
                
            }
            catch (Exception e)
            {
//              EditorUtility.DisplayDialog("Caught An Exception", e.Message + "\n" + e.StackTrace, "OK, Got it");
                Debug.LogError(e.ToString());
            
            }
        }


        private void HandleAction(ViewHandleActionArgs args)
        {
            var action = args.Action;

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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveObject(MoveAction moveAction)
        {
            Debug.Log(String.Format("MOVE OBJECT FROM: {0} to {1}",moveAction.CurrentCoordiate,moveAction.To));
            Vector3 movableStartVector = GetWorldVectorFromMapCoodinates(moveAction.CurrentCoordiate);
            Vector3 movableTargetVector = GetWorldVectorFromMapCoodinates(moveAction.To);
            GameObject movableGameObject = _movableDictionary[new Guid(moveAction.TargetId)];

            movableGameObject.GetComponent<MovableBehaviour>().startPosition = movableStartVector;
            movableGameObject.GetComponent<MovableBehaviour>().targetPosition = movableTargetVector;
            movableGameObject.GetComponent<MovableBehaviour>().animationSpeed = (float) moveAction.Duration.TotalSeconds;
            movableGameObject.GetComponent<MovableBehaviour>().isMoving = true;
            movableGameObject.GetComponent<MovableBehaviour>().journeyFract = 0;
            string animationName = movableGameObject.GetComponent<MovableBehaviour>().DecideAnimation();
            movableGameObject.GetComponent<Animator>().Play(animationName);
        }

        private void PlaceAnObject(PlaceAGameObjectAction action)
        {
            if (action == null)
                throw new ArgumentException("got a null instead of PlaceAGameObjectAction");

            switch (action.ItemType)
            {
                case ItemType.Movable:
                    GameObject newGameObject = Object.Instantiate(MovableObjectReference,
                    GetWorldVectorFromMapCoodinates(action.Coordinate)  + Vector3.back * 2, Quaternion.identity);
                    _movableDictionary.Add(action.Id,newGameObject);
                    break;
                case ItemType.Structure:
                    GameObject newStructure = Object.Instantiate(StructureObjectReference,
                    GetWorldVectorFromMapCoodinates(action.Coordinate) + Vector3.back , Quaternion.identity);
                    newStructure.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("")
                    break;
                case ItemType.Cell:
                     Object.Instantiate(CellObjectReference,
                    GetWorldVectorFromMapCoodinates(action.Coordinate) , Quaternion.identity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            



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
                var placeAMovableRequest = new PlaceAStrcutureRequest(mapCoordinate,new Coordinate(1,1,1), StructureType.Basic);
                GameViewBase.Controller.Handle(placeAMovableRequest);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log(worldPosition);
                Debug.Log(mapCoordinate.ToString());
                var placeAMovableRequest = new PlaceAMovableRequest(mapCoordinate, MovableType.NormalHuman);
                GameViewBase.Controller.Handle(placeAMovableRequest);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log(mapCoordinate.ToString());
                var gameWorldItemById = GameUniverse.GetGameWorldItemById(GameViewBase.Controller.Model);
                gameWorldItemById.ClearSelectedItems();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var viewSelectionNotification = new ViewSelectionNotification(mapCoordinate, mapCoordinate, "");
                GameViewBase.Controller.Handle(viewSelectionNotification);
            }
            if (Input.GetMouseButtonDown(1))
            {
                var rightSelectNotification = new RightClickNotification(mapCoordinate);
                GameViewBase.Controller.Handle(rightSelectNotification);
            }
            if (Input.GetMouseButtonDown(2))
            {
            
                //            _gameWorldItem.ClearSelectedItems();
            }
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

