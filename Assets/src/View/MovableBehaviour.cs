using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBehaviour : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public Vector3 currentPosition;
    public float animationSpeed = 2; //seconds
    public float journeyFract = 0; //fraction from 1 of journey
    public bool isMoving = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (isMoving)
	    {
	        journeyFract += Time.deltaTime / animationSpeed;
	        if (journeyFract > 1)
	            journeyFract = 1;
            currentPosition = Vector3.Lerp(startPosition, targetPosition, journeyFract);
	        transform.position = currentPosition;
            if (Math.Abs(journeyFract - 1) < 0.0001)
            {
                isMoving = false;
                startPosition = targetPosition;
            }
        }
//	    if (Input.GetKeyDown(KeyCode.E))
//	    {
//	        journeyFract = 0;
//	        isMoving = true;
//	    }
//        if (Input.GetMouseButton(2)) {
//            journeyFract = 0;
//            isMoving = true;
//            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            targetPosition = getCellPositionFromVector(currentMousePosition,new Vector3(0.32F,0.32F,0));
//            String animationName = DecideAnimation();
//            GetComponent<Animator>().Play(animationName);
//        }
    }

    public String DecideAnimation()
    {
        Vector3 movementDelta = targetPosition - currentPosition;
        if (movementDelta.x > 0)
            return "DottyRight";
        if (movementDelta.x < 0)
            return "DottyLeft";
        if (movementDelta.y > 0)
            return "DottyUp";
        if (movementDelta.y < 0)
            return "DottyDown";
        return "DottyIdle";
    }

    Vector3 getCellPositionFromVector(Vector3 input, Vector3 worldSizeTile) {
        input += worldSizeTile / 2;
        input.x = input.x - ((input.x) % worldSizeTile.x);
        input.y = input.y - ((input.y) % worldSizeTile.y);
        input.z = -2;
        return input;
    }
}
