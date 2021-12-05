using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    // Variables
    public float walkSpeed;
    public float behaviourTime;
    public float minimDistance;
    public int lightAttackChance = 50;
    public int blockChance = 50;
    public float movementCoolD;
    public float lightAttackCD;
    public float lastLACD;
    public float heavyAttackCD;
    public float blockTime;
    public float dodgeSpeed;
    public float dodgeCD;
    public Damage lightAttackDamage;
    public Damage heavyAttackDamage;
    public Weapon weapon;
    public AudioSource attakSound, jumpSound;
    public UnityEvent deathFinish;

    // private variables
    float attackCD = 0f;
    float movementCD = 0f;
    int attackSequence = 1;
    bool weaponDamage = false;
    bool interupable = true;
    bool isBlocking = false;
    float currentDodgeSpeed = 0f;

    Coroutine currentBehaviour = null;
    Transform player;
    Animator animator;
    Health health;
    EnemyAdaptation adaptation;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        health.death.AddListener(Death);
        adaptation = GetComponent<EnemyAdaptation>();
        ActDecision();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MovementDecision()
    {
        if (Vector3.Distance(transform.position, player.position) > minimDistance)
        {
            //Move toward player
            currentBehaviour = StartCoroutine(MoveTowardPlayer(walkSpeed));
        }
        else
        {
            //Walk side ways
            currentBehaviour = StartCoroutine(WalkSideWays(walkSpeed));
        }
    }

    void AttackDecision()
    {
        if (Vector3.Distance(transform.position, player.position) > minimDistance)
        {
            MovementDecision();
        }
        else
        {
            int attackRoll = Random.Range(0, 100);
            if (attackRoll < lightAttackChance)
            {
                // LightAttack
                LightAttack();
            }
            else
            {
                // HeavyAttack
                HeavyAttack();
            }
        }
    }

    public void ReactDecision()
    {
        if (interupable)
        {
            int reactRoll = Random.Range(0, 100);
            if (reactRoll > 40)
            {
                // Stop current movement
                if (currentBehaviour != null)
                {
                    StopCoroutine(currentBehaviour);
                    currentBehaviour = null;
                }
                reactRoll = Random.Range(0, 100);
                interupable = false;
                Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
                Vector3 playerDirection = (playerPos - transform.position).normalized;
                float angle = Mathf.Atan(playerDirection.x / playerDirection.z) * Mathf.Rad2Deg;
                if (playerDirection.z < 0f)
                {
                    angle += 180;
                }
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                if (reactRoll < blockChance)
                {
                    // Block
                    currentBehaviour = StartCoroutine(Blocking());
                }
                else
                {
                    // Dodge
                    currentBehaviour = StartCoroutine(Dodging());
                }
            }
        }

    }

    IEnumerator Blocking()
    {
        animator.SetTrigger("Block");
        isBlocking = true;
        yield return new WaitForSeconds(blockTime);
        animator.SetTrigger("SBlock");
        isBlocking = false;
        movementCD = movementCoolD;
        currentBehaviour = StartCoroutine(MovementCoolingDown());

    }

    IEnumerator Dodging()
    {
        yield return new WaitForSeconds(dodgeCD);
        animator.SetTrigger("Dodge");
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 playerDirection = (transform.position - playerPos).normalized;
        float angle = Random.Range(-90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad);
        Vector3 moveDir = RotateVectorXZ(playerDirection, angle);
        while (true)
        {
            transform.position = transform.position + moveDir * currentDodgeSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    public void DodgeChange()
    {
        if (currentDodgeSpeed == 0)
        {
            currentDodgeSpeed = dodgeSpeed;
            jumpSound.Play();
        }
        else
        {
            currentDodgeSpeed = 0;
            StopCoroutine(currentBehaviour);
            movementCD = movementCoolD;
            currentBehaviour = StartCoroutine(MovementCoolingDown());
        }
    }

    void ActDecision()
    {
        currentBehaviour = null;
        if (attackCD < 0.05f)
        {
            AttackDecision();
        }
        else
        {
            MovementDecision();
            attackCD = 0;
        }
    }

    public void GetDamage(Damage damage)
    {
        if (isBlocking && damage.blockable)
        {
            animator.SetTrigger("BlockHit");
            StartCoroutine(KnockingBack(damage.knockback, damage.knockbackTime));
            return;
        }
        // Stop current movement
        if (currentBehaviour != null)
        {
            StopCoroutine(currentBehaviour);
        }
        currentDodgeSpeed = 0;
        weaponDamage = false;
        weapon.StopDamage();
        // Receive damage
        if (!health.SurviveDamage(damage.damage))
        {
            return;
        }
        animator.SetTrigger("Hit");
        interupable = false;
        StartCoroutine(KnockingBack(damage.knockback, damage.knockbackTime));
    }

    IEnumerator KnockingBack(float speed, float time)
    {
        float currentTime = time;
        Vector3 enemyPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 moveDir = (transform.position - enemyPos);
        moveDir = new Vector3(moveDir.x, 0f, moveDir.z).normalized;
        while (currentTime > 0.05f)
        {
            transform.position = transform.position + moveDir * speed * Time.fixedDeltaTime;
            currentTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    public void AfterDamage()
    {
        movementCD = movementCoolD;
        currentBehaviour = StartCoroutine(MovementCoolingDown());
    }

    void LightAttack()
    {
        interupable = false;
        currentBehaviour = null;
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 playerDirection = (playerPos - transform.position).normalized;
        float angle = Mathf.Atan(playerDirection.x / playerDirection.z) * Mathf.Rad2Deg;
        if (playerDirection.z < 0f)
        {
            angle += 180;
        }
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        switch (attackSequence)
        {
            case 1:
                animator.SetTrigger("LA1");
                break;
            case 2:
                animator.SetTrigger("LA2");
                break;
            case 3:
                animator.SetTrigger("LA3");
                break;
        }
    }

    void HeavyAttack()
    {
        interupable = false;
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 playerDirection = (playerPos - transform.position).normalized;
        float angle = Mathf.Atan(playerDirection.x / playerDirection.z) * Mathf.Rad2Deg;
        if (playerDirection.z < 0f)
        {
            angle += 180;
        }
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        currentBehaviour = null;
        attackSequence = 4;
        animator.SetTrigger("HA");
    }

    public void SetWeaponDamage()
    {
        // Set Weapon can Damage
        if (!weaponDamage)
        {
            attakSound.Play();
            weaponDamage = true;
            if (attackSequence < 4)
            {
                weapon.SetDamage(lightAttackDamage);
            }
            else
            {
                weapon.SetDamage(heavyAttackDamage);
            }
        }
        else
        {
            float setAttackCD = 0;
            weaponDamage = false;
            weapon.StopDamage();
            switch (attackSequence)
            {
                case 1:
                    setAttackCD = lightAttackCD;
                    attackSequence++;
                    break;
                case 2:
                    setAttackCD = lightAttackCD;
                    attackSequence++;
                    break;
                case 3:
                    setAttackCD = lastLACD;
                    attackSequence = 1;
                    break;
                case 4:
                    setAttackCD = heavyAttackCD;
                    attackSequence = 1;
                    break;
            }
            movementCD = movementCoolD;
            attackCD = setAttackCD;
            currentBehaviour = StartCoroutine(MovementCoolingDown());
        }
    }

    IEnumerator MovementCoolingDown()
    {
        //animator.SetTrigger("Idle");
        interupable = true;
        while (movementCD > 0f)
        {
            movementCD -= Time.fixedDeltaTime;
            attackCD -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        ActDecision();
    }

    IEnumerator WalkSideWays(float speed)
    {
        float time = behaviourTime;
        float rad90 = Mathf.Deg2Rad * 90f;
        animator.SetTrigger("Walk");
        while (time > 0)
        {
            float delta = Time.fixedDeltaTime;
            Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 playerDirection = playerPos - transform.position;
            Vector3 moveDirection = RotateVectorXZ(playerDirection, rad90).normalized;
            float rotationAngle = Mathf.Atan(moveDirection.x / moveDirection.z) * Mathf.Rad2Deg;
            if (moveDirection.z < 0f)
            {
                rotationAngle += 180;
            }
            transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
            transform.position = transform.position + moveDirection * speed * delta;
            time -= delta;
            yield return new WaitForFixedUpdate();
        }
        movementCD = movementCoolD;
        animator.SetTrigger("SWalk");
        currentBehaviour = StartCoroutine(MovementCoolingDown());
    }

    IEnumerator MoveTowardPlayer(float speed)
    {
        animator.SetTrigger("Walk");
        while (Vector3.Distance(transform.position, player.position) > minimDistance)
        {
            Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 playerDirection = (playerPos - transform.position).normalized;
            float angle = Mathf.Atan(playerDirection.x / playerDirection.z) * Mathf.Rad2Deg;
            if (playerDirection.z < 0f)
            {
                angle += 180;
            }
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            transform.position = transform.position + playerDirection * speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        movementCD = movementCoolD;
        animator.SetTrigger("SWalk");
        currentBehaviour = StartCoroutine(MovementCoolingDown());
    }

    Vector3 RotateVectorXZ(Vector3 vector, float angle)
    {
        float cosAngle = Mathf.Cos(angle);
        float sinAngle = Mathf.Sin(angle);
        Vector3 rotated = new Vector3(vector.x * cosAngle - vector.z * sinAngle, vector.y, vector.x * sinAngle + vector.z * cosAngle);
        return rotated;
    }

    public void Death()
    {
        animator.SetTrigger("Death");
    }

    public void DeathDone()
    {
        deathFinish?.Invoke();
    }

    public void AdjustChance()
    {
        lightAttackChance = adaptation.AdjustAttackType();
        blockChance = adaptation.AdjustReactType();
        adaptation.ResetCount();
    }
}
