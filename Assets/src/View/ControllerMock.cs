using System;
using System.Collections;
using System.Collections.Generic;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Boo.Lang.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

//using UnityEngine;

public class ControllerMock : MonoBehaviour
//public class ControllerMock
{
    private GameWorldItem _gameWorldItem;
    public GameObject CellObjectReference;
    public GameObject MovableObjectReference;
    public GameObject StructureObjectReference;
    private Dictionary<Guid, GameObject> _movableDictionary = new Dictionary<Guid, GameObject>();
	// Use this for initialization
	void Start () {
        _gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 20, 2));
    }
	
	// Update is called once per frame
	void Update () {
        PlaceWaitingItems();
        CheckForKeypressesAndAddItems();
	    CheckNextMovements();
	}

    void CheckForKeypressesAndAddItems()
    {
        Vector3 worldPosition = GetMouseEffectiveWorldLocation();
        Coordinate mapCoordinate = GetMapCoordinateFromWorldVector(worldPosition);
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _gameWorldItem.CreateMovable(mapCoordinate,MovableType.NormalHuman);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            _gameWorldItem.CreateStructure(mapCoordinate, new Coordinate(1,1,1), StructureType.Basic);
        }
        if (Input.GetMouseButtonDown(0))
        {
            _gameWorldItem.SelectMovableItems(_gameWorldItem.GetMovableListInBoundary(new Boundary(mapCoordinate, mapCoordinate)));
            print(_gameWorldItem.GetSelectedIdList());
        }
        if (Input.GetMouseButtonDown(1))
        {
            foreach (MovableItem movableItem in _gameWorldItem.GetSelectedMovableItemList())
            {
                movableItem.IssueMoveCommand(mapCoordinate);
            }
        }
        if (Input.GetMouseButtonDown(2)) {
            _gameWorldItem.ClearSelectedItems();
        }
    }

    void PlaceWaitingItems()
    {
        if (_gameWorldItem.IsThereAnItemToBePlaced()) {
            print("begin placing");
            print(_gameWorldItem.GetItemsToBePlaced().Count);
            foreach (Item itemToPlace in _gameWorldItem.GetItemsToBePlaced()) {
                switch (itemToPlace.Type) {
                    case ItemType.Cell:
                        AddCellToMap(itemToPlace as CellItem);
                        break;
                    case ItemType.Movable:
                        AddMovableToMap(itemToPlace as MovableItem);
                        break;
                    case ItemType.Structure:
                        AddStructureToMap(itemToPlace as StructureItem);
                        break;
                    default:
                        throw new RuntimeException("Unexpected item type!");
                }
            }
            print("done placing");
            _gameWorldItem.ClearItemsToBePlaced();
        }
    }
    
    void AddCellToMap(CellItem cellItem)
    {
        GameObject newGameObject = Object.Instantiate(CellObjectReference, GetWorldVectorFromMapCoodinates(cellItem.Coordinate), Quaternion.identity);
    }

    void AddMovableToMap(MovableItem movableItem) {
        GameObject newGameObject = Object.Instantiate(MovableObjectReference, GetWorldVectorFromMapCoodinates(movableItem.CurrentCoordiate) + Vector3.back * 2, Quaternion.identity);
        _movableDictionary.Add(movableItem.Guid, newGameObject);
    }

    void AddStructureToMap(StructureItem structureItem) {
        GameObject newGameObject = Object.Instantiate(StructureObjectReference, GetWorldVectorFromMapCoodinates(structureItem.StructureBoundary.topLeft) + Vector3.back, Quaternion.identity);
        print("create building");
    }

    void CheckNextMovements()
    {
        foreach (MovableItem movableItem in _gameWorldItem.GetMovableItemsInMotion())
        {
            GameObject movableGameObject = _movableDictionary[movableItem.Guid];
            if (!movableGameObject.GetComponent<MovableBehaviour>().isMoving)
            {
                Vector3 movableStartVector = GetWorldVectorFromMapCoodinates(movableItem.CurrentCoordiate);
                Vector3 movableTargetVector = GetWorldVectorFromMapCoodinates(movableItem.NextCoordinate);
                movableGameObject.GetComponent<MovableBehaviour>().startPosition = movableStartVector;
                movableGameObject.GetComponent<MovableBehaviour>().targetPosition = movableTargetVector;
                movableGameObject.GetComponent<MovableBehaviour>().animationSpeed = (float)movableItem.Speed;
                movableGameObject.GetComponent<MovableBehaviour>().isMoving = true;
                movableGameObject.GetComponent<MovableBehaviour>().journeyFract = 0;
                String animationName = movableGameObject.GetComponent<MovableBehaviour>().DecideAnimation();
                movableGameObject.GetComponent<Animator>().Play(animationName);
                movableItem.MoveToNext();
            }
        }
        //                GetComponent<Animator>().Play("DottyIdle");

    }

    Vector3 GetWorldVectorFromMapCoodinates(Coordinate coordinate)
    {
        Vector3 spriteSize = GetSpriteRealSize(CellObjectReference);
        print(spriteSize);
        return new Vector3(coordinate.x * spriteSize.x, coordinate.y * spriteSize.y, 0);
    }

    Coordinate GetMapCoordinateFromWorldVector(Vector3 worldVector)
    {
        Vector3 spriteSize = GetSpriteRealSize(CellObjectReference);
        return new Coordinate((int) (worldVector.x / spriteSize.x), (int) (worldVector.y / spriteSize.y), 0);
    }

    Vector3 GetMouseEffectiveWorldLocation()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldPosition + GetSpriteRealSize(CellObjectReference) / 2;
    }

    Vector3 GetSpriteRealSize(GameObject gameObject)
    {
        Vector2 spriteSize = gameObject.GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 localSpriteSize = spriteSize / gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        return localSpriteSize;
    }
}
