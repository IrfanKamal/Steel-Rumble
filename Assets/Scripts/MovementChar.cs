using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class MovementChar : MonoBehaviour
{
    /* Base Clas for Characters */

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

    [Header("Audio")]
    public AudioSource blockSound;
    public AudioSource hitSound, deathSound;

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
    [HideInInspector]
    public UnityEvent<string> deathDone;

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
    protected readonly int idlePar = Animator.StringToHash("Idle");

    // Initiation Method
    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        animator.SetFloat(moveSpeedPar, moveSpeedMulti);
        coroutines = new List<IEnumerator>() { null, null, null, null };
        CreateWeaponLocationDictionary();
    }

    // Create Dictionary for Weapon location
    protected virtual void CreateWeaponLocationDictionary()
    {
        weaponsDic = new Dictionary<WeaponType, Transform>();
        for (int i = 0; i < weaponsLocation.Length; i++)
        {
            weaponsDic.Add(weaponsLocation[i].weaponType, weaponsLocation[i].location);
        }
    }

    // Change whether the characters can be interupted or not
    public virtual void ChangeInterupable(bool change)
    {
        interupable = change;
    }

    // Method to start walking
    public virtual void StartWalk()
    {
        if (!animator.GetBool(walkingPar))
        {
            animator.SetBool(walkingPar, true);
        }
    }

    // Method to stop walking
    public virtual void StopWalk()
    {
        animator.SetBool(walkingPar, false);
    }

    // Method to start light attack
    public virtual void LightAttack()
    {
        if (weapon && interupable)
        {
            animator.SetTrigger(lightAttackPar);
        }
    }

    // Method to start heavy attack
    public virtual void HeavyAttack()
    {
        if (weapon && interupable)
        {
            animator.SetTrigger(heavyAttackPar);
        }
    }

    // Method that called whenever the character went into attack animation
    public virtual void AttackStart(int attackSq)
    {
        coroutines[attackSqCD] = StopCor(coroutines[attackSqCD]);
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

    // Coroutine for summoning particle whenever the character start heavy attack
    protected virtual IEnumerator HeavyAttackParticle()
    {
        heavyAttackParticle.SetActive(true);
        yield return new WaitForSeconds(heavyAttackParTime);
        heavyAttackParticle.SetActive(false);
        coroutines[heavyAttackParticleCor] = null;
    }

    // Method for return to idle from attack
    public virtual void ReturnAttack()
    {
        animator.SetTrigger(returnAttackPar);
    }

    // Method to start blocking
    public virtual void StartBlock()
    {
        if (interupable)
        {
            coroutines[attackSqCD] = StopCor(coroutines[attackSqCD]);
            animator.SetBool(blockingPar, true);
        }
    }

    // Method to stop blocking
    public virtual void StopBlock()
    {
        animator.SetBool(blockingPar, false);
    }

    // Method to call character to jump
    public virtual void Jump()
    {
        if (interupable)
        {
            coroutines[attackSqCD] = StopCor(coroutines[attackSqCD]);
            animator.SetTrigger(jumpPar);
        }
    }

    // Method when character start jumping
    public virtual void StartJump()
    {
        
    }

    // Method for moving character when jumping
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

    // Method whenever the character get hit
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
                blockSound.Play();
            }
            else
            {
                GetDamage(damage, dir * -1f);
            }
        }
    }

    // Method whenever the character get damage
    public virtual void GetDamage(Damage damage, Vector3 knockDir)
    {
        StopCor();
        if (heavyAttackParticle.activeSelf)
        {
            heavyAttackParticle.SetActive(false);
        }
        weapon.DisruptAttack();
        if (health.SurviveDamage(damage.damage))
        {
            hitSound.Play();
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
            deathSound.Play();
            canMove = false;
            isDeath = true;
            animator.SetTrigger(deathPar);
        }
    }

    // Method for moving character when get damage
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

    // Method when attack animation finished
    public virtual void AttackDone()
    {
        coroutines[attackSqCD] = AttackSqCD();
        StartCoroutine(coroutines[attackSqCD]);
        interupable = true;
    }

    // Coroutine for holding characters animation attack
    protected virtual IEnumerator AttackSqCD()
    {
        yield return new WaitForSeconds(attackSqCDTime);
        coroutines[attackSqCD] = null;
        ReturnAttack();
    }

    // Method to stop all coroutines
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

    // Method to stop a spesific coroutine
    protected IEnumerator StopCor(IEnumerator coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        return null;
    }

    // Method when the character died
    public virtual void Death()
    {
        StopCor();
        interupable = false;
        isDeath = true;
        canMove = false;
        animator.SetTrigger(deathPar);
    }

    // Method to reset animator parameters
    protected virtual void ResetParameters()
    {
        if (animator.GetBool(walkingPar))
        {
            animator.SetBool(walkingPar, false);
        }
        if (animator.GetBool(blockingPar))
        {
            animator.SetBool(blockingPar, false);
        }
        attackSq = 1;
    }

    // Method to stop character from moving
    public virtual void StopMove()
    {
        canMove = false;
        ResetParameters();
        animator.SetTrigger(idlePar);
    }

    // Method to start character movement
    public virtual void StartMove()
    {
        canMove = true;
    }

    // Method when character finished the death animation
    public virtual void DeathDone()
    {
        deathDone?.Invoke(tag);
    }
}

// Class for changing frame into seconds
public static class FrametoSeconds
{
    public static float FrametoSecond(int frame, int fps)
    {
        return frame / (float)fps;
    }
}