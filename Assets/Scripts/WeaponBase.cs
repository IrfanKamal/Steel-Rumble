using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    // Class for the base of a weapon

    // Public parameters
    public RuntimeAnimatorController animationStyle;
    public WeaponType weaponType;
    public MovementChar user;
    public Transform position;
    public string targetTag;
    public float minimDistance;
    public bool damaging = false;
    [SerializeField]
    public Damage lightAttackDamage, heavyAttackDamage;
    [SerializeField]
    public int[] attackDelay;

    protected List<IEnumerator> coroutines;
    protected int currentAttackSq;
    protected IEnumerator currentAttack;


    // Methods
    protected virtual void Update()
    {
        if (position)
        {
            transform.position = position.position;
            transform.rotation = position.rotation;
            transform.localScale = position.localScale;
        }
    }

    protected virtual void Start()
    {
        if (user)
        {
            AssignWeapon(user);
        }
        coroutines = new List<IEnumerator>();
    }

    // Method to assign the weapon to characters
    public virtual void AssignWeapon(MovementChar model)
    {
        if (weaponType != WeaponType.Shield)
        {
            targetTag = model.enemyTag;
            model.weapon = this;
            model.animator.runtimeAnimatorController = animationStyle;
            model.minimDistance = minimDistance;
        }
        user = model;
        position = model.weaponsDic[weaponType];
    }

    // Method to start light attack
    public virtual void LightAttack(int attackSq)
    {
        currentAttackSq = attackSq;
    }

    // Method to start heavy attack
    public virtual void HeavyAttack()
    {
        currentAttackSq = 4;
    }

    // When the weapon hit the target
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && damaging)
        {
            damaging = false;
            MovementChar target = other.GetComponent<MovementChar>();
            DealDamage(target, transform.position);
        }
    }

    // Deal damage to the target
    public virtual void DealDamage(MovementChar target, Vector3 source)
    {
        if (currentAttackSq < 4)
        {
            target.GetHit(lightAttackDamage, source);
        }
        else
        {
            target.GetHit(heavyAttackDamage, source);
        }
    }
    
    // Method to remove weapon from player
    public virtual void DeassignWeapon()
    {
        if (user)
        {
            user.weapon = null;
            user = null;
        }
        targetTag = "";
        position = null;
    }

    // Method when attack got disrupted
    public virtual void DisruptAttack()
    {
        damaging = false;
        if (currentAttack != null)
        {
            StopCoroutine(currentAttack);
            currentAttack = null;
        }
        //StopCor();
    }

}

public enum WeaponType
{
    TwoHandSword,
    OneHandSword,
    Shield,
    Bow
}
