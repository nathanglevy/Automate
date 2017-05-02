using UnityEngine;

public class CameraDrag : MonoBehaviour {
    public float dragSpeed = 1;
    private Vector3 dragOrigin;

    void Update() {
        if (Input.GetMouseButtonDown(2)) {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2))
            return;
         
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        //Vector3 pos = Input.mousePosition - dragOrigin;
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
        //dragOrigin = Input.mousePosition;
        Camera.main.transform.Translate(move, Space.World);
    }
}
