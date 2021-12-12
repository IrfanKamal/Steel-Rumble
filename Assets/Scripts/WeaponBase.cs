using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
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

    public virtual void AssignWeapon(MovementChar model)
    {
        if (weaponType != WeaponType.Shield)
        {
            user = model;
            targetTag = model.enemyTag;
            model.weapon = this;
            model.animator.runtimeAnimatorController = animationStyle;
            model.minimDistance = minimDistance;
        }
        position = model.weaponsDic[weaponType];
    }

    public virtual void LightAttack(int attackSq)
    {
        currentAttackSq = attackSq;
    }

    public virtual void HeavyAttack()
    {
        currentAttackSq = 4;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && damaging)
        {
            damaging = false;
            MovementChar target = other.GetComponent<MovementChar>();
            DealDamage(target, transform.position);
        }
    }

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

    public virtual void DeassignWeapon()
    {
        user = null;
        targetTag = "";
        position = null;
    }

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

    protected void StopCor()
    {
        for (int i = 0; i < coroutines.Count; i++)
        {
            if (coroutines[i] != null)
            {
                StopCoroutine(coroutines[i]);
                coroutines[i] = null;
            }
        }
    }

    protected void StopCor(IEnumerator coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}

public enum WeaponType
{
    TwoHandSword,
    OneHandSword,
    Shield,
    Bow
}
