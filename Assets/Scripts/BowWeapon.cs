using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowWeapon : WeaponBase
{
    //IEnumerator currentAttack;
    public ObjectPool arrowPool;
    public Transform impactPoint;

    protected override void Start()
    {
        base.Start();
        coroutines.Add(currentAttack);
        foreach (GameObject objectPool in GameObject.FindGameObjectsWithTag("ObjectPool"))
        {
            ObjectPool script = objectPool.GetComponent<ObjectPool>();
            if (script.objectName == "Arrow")
            {
                arrowPool = script;
                break;
            }
        }
    }

    public override void LightAttack(int attackSq)
    {
        base.LightAttack(attackSq);
        float delay = FrametoSeconds.FrametoSecond(attackDelay[attackSq - 1], 30);
        currentAttack = AttackDelay(delay);
        StartCoroutine(currentAttack);
    }

    public override void HeavyAttack()
    {
        base.HeavyAttack();
        float delay = FrametoSeconds.FrametoSecond(attackDelay[3], 30);
        currentAttack = AttackDelay(delay);
        StartCoroutine(currentAttack);
    }

    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrow();
        currentAttack = null;
    }

    void SpawnArrow()
    {
        Transform arrow = arrowPool.RequestObject().transform;
        arrow.position = impactPoint.position;
        float angle = user.transform.rotation.eulerAngles.y;
        arrow.rotation = Quaternion.Euler(90f, 0f, -angle);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrow.gameObject.SetActive(true);
        arrowScript.moving = true;
        arrowScript.weapon = this;
        arrowScript.DetermineMoveDir();
        if (currentAttackSq == 4)
        {
            arrowScript.heavyAttack = true;
        }
        else
        {
            arrowScript.heavyAttack = false;
        }
    }
}
