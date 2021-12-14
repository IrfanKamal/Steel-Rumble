using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnemy : MovementChar
{
    // Class for enemy characters

    // Enemy exclusives variables
    [Header("Enemy Exclusives")]
    public float lightAttackCD;
    public float heavyAttackCD;
    public float minimMoveTowardPlayer = 0.3f;
    public float blockTime;
    public float walkSideWayTime;
    public int lightAttackChance = 50;
    public int dodgeChance = 50;
    public int notReacting = 40;
    public Transform player;
    public MovementPlayer playerScript;

    // Private parameters
    int curBehaviour;
    bool canAttack = true;

    protected override void Awake()
    {
        base.Awake();
        coroutines.Add(null);
        curBehaviour = coroutines.Count - 1;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<MovementPlayer>();
        playerScript.attack.AddListener(ReactAttack);
    }

    // Set where is the player
    public void AssignPlayer(MovementPlayer player)
    {
        playerScript = player;
        this.player = player.transform;
    }

    public override void ChangeInterupable(bool change)
    {
        base.ChangeInterupable(change);
        if (change && canMove)
        {
            if (attackSq == 3)
            {
                StartCoroutine(AttackCD(lightAttackCD));
                attackSq = 1;
            }
            else if (attackSq == 4)
            {
                StartCoroutine(AttackCD(heavyAttackCD));
                attackSq = 1;
            }
            ActDecision();
        }
    }

    public override void AttackDone()
    {
        interupable = true;
        if (canMove)
        {
            if (Vector3.Distance(transform.position, player.position) < minimDistance)
            {
                AttackDecision();
            }
            else
            {
                animator.SetTrigger(returnAttackPar);
                MovementDecision(false);
            }
        }
        else
        {
            animator.SetTrigger(returnAttackPar);
        }
    }

    public override void GetDamage(Damage damage, Vector3 knockDir)
    {
        base.GetDamage(damage, knockDir);
        ResetParameters();
    }

    public override void StartJump()
    {
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 playerDirection = (transform.position - playerPos).normalized;
        float angle = Random.Range(-90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad);
        Vector3 moveDir = RotateVectorXZ(playerDirection, angle);
        coroutines[jumping] = Jumping(moveDir);
        StartCoroutine(coroutines[jumping]);
    }

    // Method to react when player is attacking
    public void ReactAttack(int attackSq)
    {
        if (interupable && Vector3.Distance(transform.position, player.position) < playerScript.minimDistance && canMove)
        {
            if (!GetChance(notReacting))
            {
                StopCor();
                ResetParameters();
                FacePlayer();
                if (GetChance(dodgeChance))
                {
                    Jump();
                }
                else
                {
                    coroutines[curBehaviour] = Blocking();
                    StartCoroutine(coroutines[curBehaviour]);
                }
            }
        }
    }

    // Coroutine for how long the character will be blocking
    IEnumerator Blocking()
    {
        animator.SetBool(blockingPar, true);
        yield return new WaitForSeconds(blockTime);
        animator.SetBool(blockingPar, false);
        coroutines[curBehaviour] = null;
    }

    // Method to decide what to do
    void ActDecision()
    {
        if (Vector3.Distance(transform.position, player.position) < minimDistance)
        {
            if (canAttack)
            {
                AttackDecision();
            }
            else
            {
                MovementDecision(true);
            }
        }
        else
        {
            MovementDecision(false);
        }
    }

    // Method to decide what movement to do
    void MovementDecision(bool inDistance)
    {
        if (inDistance)
        {
            coroutines[curBehaviour] = WalkSideWays();
            StartCoroutine(coroutines[curBehaviour]);
        }
        else
        {
            coroutines[curBehaviour] = MoveTowardPlayer();
            StartCoroutine(coroutines[curBehaviour]);
        }
    }

    // Method to decide what attack to do
    void AttackDecision()
    {
        FacePlayer();
        if (GetChance(lightAttackChance))
        {
            LightAttack();
        }
        else
        {
            HeavyAttack();
        }
    }

    // Method to cooldown after attack
    IEnumerator AttackCD(float cd)
    {
        canAttack = false;
        yield return new WaitForSeconds(cd);
        canAttack = true;
    }

    // Method to draw chance
    bool GetChance(int chance)
    {
        int random = Random.Range(0, 100);
        if (random < chance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Method to face the player
    void FacePlayer()
    {
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 playerDirection = (playerPos - transform.position).normalized;
        float angle = Mathf.Atan(playerDirection.x / playerDirection.z) * Mathf.Rad2Deg;
        if (playerDirection.z < 0f)
        {
            angle += 180;
        }
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    // Coroutine behaviour to move toward player
    IEnumerator MoveTowardPlayer()
    {
        float time = minimMoveTowardPlayer;
        animator.SetBool(walkingPar, true);
        while (Vector3.Distance(transform.position, player.position) > minimDistance || time > 0f)
        {
            FacePlayer();
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool(walkingPar, false);
        coroutines[curBehaviour] = null;
    }

    // Coroutine behaviour to walk sideways from player
    IEnumerator WalkSideWays()
    {
        float time = walkSideWayTime;
        float minimTime = minimMoveTowardPlayer;
        int reverse = Random.Range(0, 2);
        if (reverse == 0)
        {
            reverse = -1;
        }
        float rad90 = Mathf.Deg2Rad * 90f * reverse;
        animator.SetBool(walkingPar, true);
        while (time > 0f || minimTime > 0f)
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
            time -= delta;
            minimTime -= delta;
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool(walkingPar, false);
        coroutines[curBehaviour] = null;
    }

    public override void StartMove()
    {
        base.StartMove();
        ActDecision();
    }

    // Method to rotate a vector at x and z
    Vector3 RotateVectorXZ(Vector3 vector, float angle)
    {
        float cosAngle = Mathf.Cos(angle);
        float sinAngle = Mathf.Sin(angle);
        Vector3 rotated = new Vector3(vector.x * cosAngle - vector.z * sinAngle, vector.y, vector.x * sinAngle + vector.z * cosAngle);
        return rotated;
    }
}
