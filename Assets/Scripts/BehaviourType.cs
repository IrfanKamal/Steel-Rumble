using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourType
{
    Idle,
    Walk,
    Jump,
    LightAttack,
    HeavyAttack,
    Block,
    Hit,
    Death
}

public struct Behaviour
{
    public BehaviourType behaviourType;
    public bool interupable;

    public Behaviour(BehaviourType type, bool interupable)
    {
        behaviourType = type;
        this.interupable = interupable;
    }
}
