using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
    public GameObject TilePrefabGameObject;
    public GameObject MovablePrefabGameObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 spriteSize = TilePrefabGameObject.GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 localSpriteSize = spriteSize / TilePrefabGameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        Vector3 worldSize = localSpriteSize;
        worldSize.x *= transform.lossyScale.x;
        worldSize.y *= transform.lossyScale.y;
        if (Input.GetKeyDown(KeyCode.Q) == true)
	    {
            Debug.Log("Pressed Q - Generating map items");
            GameObject mapParentObject = new GameObject("GameWorldMapContainer");

            for (int x = 0; x < 10; x++)
	        {
	            for (int y = 0; y < 20; y++)
	            {

//	                Vector3 screenSize = 0.5f * worldSize / Camera.main.orthographicSize;
//	                screenSize.y *= Camera.main.aspect;
//
//                    Debug.Log( "localSpriteSize: " + localSpriteSize);
//                    Debug.Log( "worldSize: " + worldSize);
//                    Debug.Log( "screenSize: " + screenSize);
	                GameObject newGameObject = Instantiate(TilePrefabGameObject, new Vector3(x * localSpriteSize.x, y * localSpriteSize.y, 0), Quaternion.identity);
	                newGameObject.name = "MapCell_" + x + "_" + y;
                    newGameObject.transform.parent = mapParentObject.transform;
	            }
	        }
        }

	    if (Input.GetKeyDown(KeyCode.W) == true)
	    {
            Debug.Log("Pressed W - Generating a movable at mouse");
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject newGameObject = Instantiate(MovablePrefabGameObject, getCellPositionFromVector(currentMousePosition, worldSize), Quaternion.identity);
        }

    }

    Vector3 getCellPositionFromVector(Vector3 input, Vector3 worldSizeTile)
    {
        input += worldSizeTile / 2;
        input.x = input.x - ((input.x ) % worldSizeTile.x);
        input.y = input.y - ((input.y ) % worldSizeTile.y);
        input.z = -2;
        return input;
    }
}
