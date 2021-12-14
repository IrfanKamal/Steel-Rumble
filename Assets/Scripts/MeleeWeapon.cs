using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    // Class for melee weapons

    // Variables
    [Header("Melee Weapons Exclusives")]
    [SerializeField]
    public int[] attackLength;
    public ObjectPool heavyAttackParticlePool;
    public float heavyAttackRotationOffset;
    public Transform impactPoint;


    protected override void Start()
    {
        base.Start();
        coroutines.Add(currentAttack);
        foreach (GameObject objectPool in GameObject.FindGameObjectsWithTag("ObjectPool"))
        {
            ObjectPool script = objectPool.GetComponent<ObjectPool>();
            if (script.objectName == "MeleeHeavyAttack")
            {
                heavyAttackParticlePool = script;
                break;
            }
        }
    }

    // Melee weapon light attack
    public override void LightAttack(int attackSq)
    {
        base.LightAttack(attackSq);
        float delay = FrametoSeconds.FrametoSecond(attackDelay[attackSq - 1], 30);
        float length = FrametoSeconds.FrametoSecond(attackLength[attackSq - 1], 30);
        currentAttack = LightAttackDelay(delay, length);
        StartCoroutine(currentAttack);
    }

    // Melee weapon heavy attack
    public override void HeavyAttack()
    {
        base.HeavyAttack();
        currentAttack = HeavyAttackDelay();
        StartCoroutine(currentAttack);
    }

    // Coroutine to count the delay from light attack
    IEnumerator LightAttackDelay(float delay, float length)
    {
        yield return new WaitForSeconds(delay);
        damaging = true;
        yield return new WaitForSeconds(length);
        damaging = false;
        currentAttack = null;
    }

    // Coroutine to count the delay from heavy attack
    IEnumerator HeavyAttackDelay()
    {
        float delay = FrametoSeconds.FrametoSecond(attackDelay[3], 30);
        yield return new WaitForSeconds(delay);
        Transform heavyAttackParticle = heavyAttackParticlePool.RequestObject().transform;
        heavyAttackParticle.position = impactPoint.position;
        float angle = user.transform.rotation.eulerAngles.y + heavyAttackRotationOffset;
        heavyAttackParticle.rotation = Quaternion.Euler(-90f, 0f, angle);
        heavyAttackParticle.gameObject.SetActive(true);
        heavyAttackParticle.GetComponent<HeavyAttackParticle>().AssignParticle(this);
        currentAttack = null;
    }
}
