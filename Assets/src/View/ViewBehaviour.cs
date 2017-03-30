using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBehaviour : MonoBehaviour {

    public RectTransform selectionBox;
    private Vector2 initialClickPosition = Vector2.zero;
    public Camera mainCamera;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 mousePosition = Input.mousePosition;

	    if (Input.GetMouseButtonDown(0))
	    {
            Debug.Log("Current position of rect: " + selectionBox.anchoredPosition + " delta: " + selectionBox.sizeDelta);
            Debug.Log("Pressed left click. Mouse position: " + mousePosition);

            // Get the initial click position of the mouse. No need to convert to GUI space
            // since we are using the lower left as anchor and pivot.
            
            initialClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //initialClickPosition = Camera.main.ViewportToWorldPoint(initialClickPosition);
            // The anchor is set to the same place.
            selectionBox.anchoredPosition = initialClickPosition;
        }

        // While we are dragging.
        if (Input.GetMouseButton(0)) {
            // Store the current mouse position in screen space.
            Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //currentMousePosition = Camera.main.ViewportToWorldPoint(currentMousePosition);
            // How far have we moved the mouse?
            Vector2 difference = currentMousePosition - initialClickPosition;

            // Copy the initial click position to a new variable. Using the original variable will cause
            // the anchor to move around to wherever the current mouse position is,
            // which isn't desirable.
            Vector2 startPoint = initialClickPosition;

            // The following code accounts for dragging in various directions.
            if (difference.x < 0) {
                startPoint.x = currentMousePosition.x;
                difference.x = -difference.x;
            }
            if (difference.y < 0) {
                startPoint.y = currentMousePosition.y;
                difference.y = -difference.y;
            }

            //print("currentMouse" + currentMousePosition);
            //print("StartAnchor" + startPoint);
            //print("Diff" + difference);
            // Set the anchor, width and height every frame.
            selectionBox.anchoredPosition = startPoint;
            //selectionBox.anchoredPosition = Camera.main.ViewportToWorldPoint(startPoint);
            selectionBox.sizeDelta = difference;
            //selectionBox.sizeDelta = Camera.main.ViewportToWorldPoint(difference);
        }

        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Released left click. Mouse position: " + mousePosition);
            Debug.Log("Selected Rectangle Was: " + initialClickPosition + " : " + mousePosition);

            // Reset
            initialClickPosition = Vector2.zero;
            selectionBox.anchoredPosition = Vector2.zero;
            selectionBox.sizeDelta = Vector2.zero;
        }


        if (Input.GetMouseButtonDown(1))
            Debug.Log("Pressed right click. Mouse position: " + mousePosition);

        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click. Mouse position: " + mousePosition);

        if (Input.GetMouseButtonUp(1))
            Debug.Log("Released right click. Mouse position: " + mousePosition);

        if (Input.GetMouseButtonUp(2))
            Debug.Log("Released middle click. Mouse position: " + mousePosition);
    }

    private Rect GetSelectionRect(Vector2 start, Vector2 end) {
        int width = (int)(end.x - start.x);
        int height = (int)((Screen.height - end.y) - (Screen.height - start.y));
        return (new Rect(start.x, Screen.height - start.y, width, height));
    }
}
