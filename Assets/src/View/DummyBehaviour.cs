using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
    public GameObject prefabGameObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Q) == true)
	    {
            Debug.Log("Pressed Q - Generating map items");
            GameObject mapParentObject = new GameObject("GameWorldMapContainer");

            for (int x = 0; x < 10; x++)
	        {
	            for (int y = 0; y < 10; y++)
	            {
	                Vector2 spriteSize = prefabGameObject.GetComponent<SpriteRenderer>().sprite.rect.size;
                    Vector2 localSpriteSize = spriteSize / prefabGameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
                    Vector3 worldSize = localSpriteSize;
                    worldSize.x *= transform.lossyScale.x;
                    worldSize.y *= transform.lossyScale.y;
//	                Vector3 screenSize = 0.5f * worldSize / Camera.main.orthographicSize;
//	                screenSize.y *= Camera.main.aspect;
//
//                    Debug.Log( "localSpriteSize: " + localSpriteSize);
//                    Debug.Log( "worldSize: " + worldSize);
//                    Debug.Log( "screenSize: " + screenSize);
	                GameObject newGameObject = Instantiate(prefabGameObject, new Vector3(x * localSpriteSize.x, y * localSpriteSize.y, 0), Quaternion.identity);
	                newGameObject.name = "MapCell_" + x + "_" + y;
                    newGameObject.transform.parent = mapParentObject.transform;
	            }
	        }
        }

    }
}
