using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterupableBehaviour : StateMachineBehaviour
{
    public bool interupableEnter, changeExit;

    MovementChar character;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponent<MovementChar>();
        character.ChangeInterupable(interupableEnter);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (changeExit)
            character.ChangeInterupable(!interupableEnter);
    }
}
