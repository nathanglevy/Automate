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
using UnityEngine;
using Object = UnityEngine.Object;

namespace src.View
{
    public class UnityViewProxy : MonoBehaviour
    {

        // Use this for initialization
        private static GameViewBase _gameViewBase;
        public Dictionary<Guid, GameObject> _movableDictionary = new Dictionary<Guid, GameObject>();
        // Game Objects
        public GameObject CellObjectReference;
        public GameObject MovableObjectReference;
        public GameObject StructureObjectReference;

        public void Start()
        {

            _gameViewBase = new GameViewBase();

            _gameViewBase.OnActionReady += HandleAction;

            var gameController = new GameController(_gameViewBase);

            // notify that Start being Done Now
            _gameViewBase.PerformOnStart();

        }

        public void Update()
        {
            try
            {
                CheckForKeypressesAndAddItems();

                _gameViewBase.PerformCompleteUpdate();
            }
            catch (Exception e)
            {
//            EditorUtility.DisplayDialog("Caught An Exception", e.Message + "\n" + e.StackTrace, "OK, Got it");
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
            Vector3 movableStartVector = GetWorldVectorFromMapCoodinates(moveAction.CurrentCoordiate);
            Vector3 movableTargetVector = GetWorldVectorFromMapCoodinates(moveAction.To);
            GameObject movableGameObject = _movableDictionary[new Guid(moveAction.TargetId)];

            movableGameObject.GetComponent<MovableBehaviour>().startPosition = movableStartVector;
            movableGameObject.GetComponent<MovableBehaviour>().targetPosition = movableTargetVector;
            movableGameObject.GetComponent<MovableBehaviour>().animationSpeed = (float) moveAction.Duration.TotalMilliseconds;
//            movableGameObject.GetComponent<MovableBehaviour>().animationSpeed = (float) 10;
            movableGameObject.GetComponent<MovableBehaviour>().isMoving = true;
            movableGameObject.GetComponent<MovableBehaviour>().journeyFract = 0;
            string animationName = movableGameObject.GetComponent<MovableBehaviour>().DecideAnimation();
            movableGameObject.GetComponent<Animator>().Play(animationName);
        }

        private void PlaceAnObject(PlaceAGameObjectAction action)
        {
            if (action == null)
                throw new ArgumentException("got a null instead of PlaceAGameObjectAction");
            GameObject gameObject = null;

            switch (action.ItemType)
            {
                case ItemType.Movable:
                    gameObject = MovableObjectReference;
                    _movableDictionary.Add(action.Id, MovableObjectReference);
                    break;
                case ItemType.Structure:
                    gameObject = StructureObjectReference;
                    break;
                case ItemType.Cell:
                    gameObject = CellObjectReference;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            GameObject newGameObject = Object.Instantiate(gameObject,
                GetWorldVectorFromMapCoodinates(action.Coordinate), Quaternion.identity);


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
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var placeAMovableRequest = new PlaceAMovableRequest(mapCoordinate, MovableType.NormalHuman);
                _gameViewBase.Controller.Handle(placeAMovableRequest);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var placeAMovableRequest = new PlaceAStrcutureRequest(mapCoordinate,new Coordinate(1,1,1), StructureType.Basic);
                _gameViewBase.Controller.Handle(placeAMovableRequest);
            }
            if (Input.GetMouseButtonDown(0))
            {
                var viewSelectionNotification = new ViewSelectionNotification(new Coordinate(0,0,0), new Coordinate(10,10,0), "");
                _gameViewBase.Controller.Handle(viewSelectionNotification);
            }
            if (Input.GetMouseButtonDown(1))
            {
                var rightSelectNotification = new RightClickNotification(mapCoordinate);
                _gameViewBase.Controller.Handle(rightSelectNotification);
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

