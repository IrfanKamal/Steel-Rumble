using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingBehavior : StateMachineBehaviour
{
    float moveSpeed;
    Rigidbody rb;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //rb.AddForce(Vector3.back * moveSpeed);
    }
}
