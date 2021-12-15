using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChar : MonoBehaviour
{
    public Transform target;
    public float maxX, maxZ, minX, minZ;

    Vector3 offsetFromChar;
    private void Start()
    {
        offsetFromChar = transform.position - target.position;
    }
    // Update is called once per frame
    void Update()
    {
        // Camera Border
        Vector3 cameraPosition = target.position + offsetFromChar;
        if(cameraPosition.x < minX)
        {
            cameraPosition.x = minX;
        } else
        if(cameraPosition.x > maxX)
        {
            cameraPosition.x = maxX;
        }

        if(cameraPosition.z < minZ)
        {
            cameraPosition.z = minZ;
        } else
        if(cameraPosition.z > maxZ)
        {
            cameraPosition.z = maxZ;
        }
        transform.position = cameraPosition;
    }
}
