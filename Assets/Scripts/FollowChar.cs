using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChar : MonoBehaviour
{
    public Transform target;

    Vector3 offsetFromChar;
    private void Start()
    {
        offsetFromChar = transform.position - target.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offsetFromChar;
    }
}
