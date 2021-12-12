using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class MovementChar : MonoBehaviour
{
    // Public Variables
    public bool canMove;
    public float moveSpeedMulti = 1;
    public float jumpSpeed;
    public int jumpFrame;
    public float attackSqCDTime;
    public string enemyTag;
    public float minimDistance;
    [Header("Weapon")]
    public WeaponBase weapon;

    [System.Serializable]
    public struct WeaponLocation
    {
        public WeaponType weaponType;
        public Transform location;
    }
    [SerializeField]
    public WeaponLocation[] weaponsLocation;
    public Dictionary<WeaponType, Transform> weaponsDic;
    public GameObject heavyAttackParticle;
    public float heavyAttackParTime;

    // Private Variables
    protected Behaviour currentBehaviour;
    protected int attackSqCD = 0;
    protected int jumping = 1;
    protected int knockedBack = 2;
    protected int heavyAttackParticleCor = 3;
    protected List<IEnumerator> coroutines;
    [HideInInspector]
    public bool interupable = true;
    protected bool isDeath = false;
    protected int attackSq = 1;

    // Component
    [HideInInspector]
    public Animator animator;
    protected Rigidbody rb;
    protected Health health;

    // Animator Parameters
    protected readonly int walkingPar = Animator.StringToHash("Walking");
    protected readonly int moveSpeedPar = Animator.StringToHash("MoveSpeed");
    protected readonly int jumpPar = Animator.StringToHash("Jump");
    protected readonly int lightAttackPar = Animator.StringToHash("LightAttack");
    protected readonly int heavyAttackPar = Animator.StringToHash("HeavyAttack");
    protected readonly int returnAttackPar = Animator.StringToHash("AttackReturn");
    protected readonly int blockingPar = Animator.StringToHash("Blocking");
    protected readonly int blockHitPar = Animator.StringToHash("BlockHit");
    protected readonly int hitPar = Animator.StringToHash("Hit");
    protected readonly int deathPar = Animator.StringToHash("Death");

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        animator.SetFloat(moveSpeedPar, moveSpeedMulti);
        coroutines = new List<IEnumerator>() { null, null, null, null };
        CreateWeaponLocationDictionary();
    }

    protected virtual void CreateWeaponLocationDictionary()
    {
        weaponsDic = new Dictionary<WeaponType, Transform>();
        for (int i = 0; i < weaponsLocation.Length; i++)
        {
            weaponsDic.Add(weaponsLocation[i].weaponType, weaponsLocation[i].location);
        }
    }

    public virtual void ChangeInterupable(bool change)
    {
        interupable = change;
    }

    public virtual void StartWalk()
    {
        if (!animator.GetBool(walkingPar))
        {
            animator.SetBool(walkingPar, true);
        }
    }

    public virtual void StopWalk()
    {
        animator.SetBool(walkingPar, false);
    }

    public virtual void LightAttack()
    {
        if (weapon && interupable)
        {
            animator.SetTrigger(lightAttackPar);
        }
    }

    public virtual void HeavyAttack()
    {
        if (weapon && interupable)
        {
            animator.SetTrigger(heavyAttackPar);
        }
    }

    public virtual void AttackStart(int attackSq)
    {
        StopCor(coroutines[attackSqCD]);
        this.attackSq = attackSq;
        if (attackSq < 4)
        {
            weapon.LightAttack(attackSq);
        }
        else
        {
            coroutines[heavyAttackParticleCor] = HeavyAttackParticle();
            StartCoroutine(coroutines[heavyAttackParticleCor]);
            weapon.HeavyAttack();
        }
    }

    protected virtual IEnumerator HeavyAttackParticle()
    {
        heavyAttackParticle.SetActive(true);
        yield return new WaitForSeconds(heavyAttackParTime);
        heavyAttackParticle.SetActive(false);
        coroutines[heavyAttackParticleCor] = null;
    }

    public virtual void ReturnAttack()
    {
        animator.SetTrigger(returnAttackPar);
    }

    public virtual void StartBlock()
    {
        if (interupable)
        {
            StopCor(coroutines[attackSqCD]);
            animator.SetBool(blockingPar, true);
        }
    }

    public virtual void StopBlock()
    {
        animator.SetBool(blockingPar, false);
    }

    public virtual void Jump()
    {
        if (interupable)
        {
            StopCor(coroutines[attackSqCD]);
            animator.SetTrigger(jumpPar);
        }
    }

    public virtual void StartJump()
    {
        
    }

    protected virtual IEnumerator Jumping(Vector3 moveDir)
    {
        float time = FrametoSeconds.FrametoSecond(jumpFrame, 30);
        while (time > 0f)
        {
            rb.AddForce(moveDir * jumpSpeed);
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        coroutines[jumping] = null;
    }

    public virtual void GetHit(Damage damage, Vector3 source)
    {
        if (!isDeath)
        {
            Vector3 dir = new Vector3(source.x - transform.position.x, 0f, source.z - transform.position.z);
            float angle = Mathf.Atan(dir.x / dir.z) * Mathf.Rad2Deg;
            if (dir.z < 0f)
                angle += 180;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            if (animator.GetBool(blockingPar) && damage.blockable)
            {
                animator.SetTrigger(blockHitPar);
            }
            else
            {
                GetDamage(damage, dir * -1f);
            }
        }
    }

    public virtual void GetDamage(Damage damage, Vector3 knockDir)
    {
        StopCor();
        weapon.DisruptAttack();
        if (health.SurviveDamage(damage.damage))
        {
            if (heavyAttackParticle.activeSelf == true)
            {
                heavyAttackParticle.SetActive(false);
            }
            animator.SetTrigger(hitPar);
            coroutines[knockedBack] = KnockingBack(damage, knockDir);
            StartCoroutine(coroutines[knockedBack]);
        }
        else
        {
            canMove = false;
            isDeath = true;
            animator.SetTrigger(deathPar);
        }
    }

    protected virtual IEnumerator KnockingBack(Damage damage, Vector3 knockDir)
    {
        float time = damage.knockbackTime;
        while(time > 0f)
        {
            rb.AddForce(knockDir * damage.knockback);
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        coroutines[knockedBack] = null;
    }

    public virtual void AttackDone()
    {
        coroutines[attackSqCD] = AttackSqCD();
        StartCoroutine(coroutines[attackSqCD]);
        interupable = true;
    }

    protected virtual IEnumerator AttackSqCD()
    {
        yield return new WaitForSeconds(attackSqCDTime);
        coroutines[attackSqCD] = null;
        ReturnAttack();
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

    public virtual void Death()
    {
        StopCor();
        interupable = false;
        isDeath = true;
        canMove = false;
        animator.SetTrigger(deathPar);
    }
}

public static class FrametoSeconds
{
    public static float FrametoSecond(int frame, int fps)
    {
        return frame / (float)fps;
    }
}