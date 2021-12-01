using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    public Vector3 pushDirection;
    public float pushStrength;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        Transform colider = other.transform;
        colider.position = colider.position + pushDirection * pushStrength * Time.fixedDeltaTime;
    }
}
