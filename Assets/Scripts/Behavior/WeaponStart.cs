using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStart : StateMachineBehaviour
{
    public int attackSequence;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<MovementChar>().AttackStart(attackSequence);
    }
}
