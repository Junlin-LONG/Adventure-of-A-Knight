using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Outlet
    public Transform playerTransform;

    // Configurations
    private float upperLowerBound;
    private float leftRightBound;

    private void Start()
    {
        // Moving bounds of camera
        upperLowerBound = GameController.instance.mapWidth / 2 - GetComponent<Camera>().orthographicSize;
        leftRightBound = GameController.instance.mapLength / 2 - GetComponent<Camera>().orthographicSize * 16 / 9;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = new(0, 0, -10);

        if (playerTransform.position.y >= upperLowerBound)
            cameraPosition.y = upperLowerBound;
        else if (playerTransform.position.y <= -upperLowerBound)
            cameraPosition.y = -upperLowerBound;
        else
            cameraPosition.y = playerTransform.position.y;

        if (playerTransform.position.x >= leftRightBound)
            cameraPosition.x = leftRightBound;
        else if (playerTransform.position.x <= -leftRightBound)
            cameraPosition.x = -leftRightBound;
        else
            cameraPosition.x = playerTransform.position.x;

        transform.position = cameraPosition;
    }

}
