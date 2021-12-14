using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    // NOT USED
    // Variables
    public float moveSpeed;
    public float attakSqTime;
    public float dodgingSpeed;
    public float dodgeInit;
    public Damage lightAttackDamage;
    public Damage heavyAttackDamage;
    public Weapon weapon;
    public AudioSource attackSound;
    public AudioSource jumpSound;
    public UnityEvent deathFinish;

    // Private Variables
    bool canMove = true;
    bool canAttack = true;
    int attackSq = 1;
    bool weaponDamage = false;
    float dodgeSpeed = 0;
    bool isBlocking = false;

    Animator animator;
    Transform enemy;
    EnemyMovement enemyScript;
    Coroutine dodging = null;
    Coroutine attackSqCD = null;
    Health health;
    EnemyAdaptation enemyAdaptation;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
        enemyScript = enemy.GetComponent<EnemyMovement>();
        health = GetComponent<Health>();
        //health.death.AddListener(Death);
        enemyAdaptation = enemy.GetComponent<EnemyAdaptation>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
        AttackInput();
        DodgeInput();
        BlockInput();
    }

    void MoveInput()
    {
        if (canMove && !isBlocking)
        {
            Vector3 moveDirection = GetKeyboardDirection();
            if (moveDirection != Vector3.zero)
            {
                animator.SetBool("Walking", true);
                float angle = Mathf.Atan(moveDirection.x / moveDirection.z) * Mathf.Rad2Deg;
                if (moveDirection.z < 0f)
                {
                    angle += 180;
                }
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                transform.position = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            }
            else
            {
                animator.SetBool("Walking", false);
            }
        }
    }
    void AttackInput()
    {
        if (canAttack && !isBlocking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                enemyScript.ReactDecision();
                enemyAdaptation.CountPlayerAttack(attackSq);
                animator.SetBool("Walking", false);
                canMove = false;
                canAttack = false;
                switch (attackSq)
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
            if (Input.GetMouseButtonDown(1))
            {
                enemyScript.ReactDecision();
                enemyAdaptation.CountPlayerAttack(attackSq);
                animator.SetBool("Walking", false);
                canMove = false;
                canAttack = false;
                animator.SetTrigger("HA");
                attackSq = 4;
            }
        }
    }

    public void SetWeaponDamage()
    {
        // Set Weapon can Damage
        if (!weaponDamage)
        {
            attackSound.Play();
            weaponDamage = true;
            if (attackSq < 4)
            {
                weapon.SetDamage(lightAttackDamage);
            }
            else
                weapon.SetDamage(heavyAttackDamage);
        }
        else
        {
            weaponDamage = false;
            weapon.StopDamage();
            switch (attackSq)
            {
                case 1:
                    attackSq++;
                    break;
                case 2:
                    attackSq++;
                    break;
                case 3:
                    attackSq = 1;
                    break;
                case 4:
                    attackSq = 1;
                    break;
            }
            if (attackSqCD != null)
            {
                StopCoroutine(attackSqCD);
            }
            attackSqCD = StartCoroutine(AttackSqCD());
            canAttack = true;
            canMove = true;
        }
    }

    IEnumerator AttackSqCD()
    {
        yield return new WaitForSeconds(attakSqTime);
        attackSqCD = null;
        attackSq = 1;
    }

    Vector3 GetKeyboardDirection()
    {
        Vector3 moveDirection = Vector3.zero;
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        if (horizontalMovement > 0.05f)
        {
            moveDirection += Vector3.left;
        }
        else if (horizontalMovement < -0.05f)
        {
            moveDirection += Vector3.right;
        }
        if (verticalMovement > 0.05f)
        {
            moveDirection += Vector3.back;
        }
        else if (verticalMovement < -0.05f)
        {
            moveDirection += Vector3.forward;
        }
        return moveDirection.normalized;
    }

    void DodgeInput()
    {
        if (canMove && Input.GetKeyDown(KeyCode.Space) && !isBlocking)
        {
            Vector3 moveDirection = GetKeyboardDirection();
            if (moveDirection == Vector3.zero)
            {
                return;
            }
            animator.SetBool("Walking", false);
            canMove = false;
            canAttack = false;
            dodging = StartCoroutine(DodgingMove(moveDirection));
        }
    }

    public void DodgeChange()
    {
        if (dodgeSpeed == 0)
        {
            jumpSound.Play();
            dodgeSpeed = dodgingSpeed;
        }
        else
        {
            dodgeSpeed = 0;
            canMove = true;
            canAttack = true;
            StopCoroutine(dodging);
            dodging = null;
        }
    }

    IEnumerator DodgingMove(Vector3 direction)
    {
        yield return new WaitForSeconds(dodgeInit);
        animator.SetTrigger("Dodge");
        while (true)
        {
            transform.position = transform.position + direction * dodgeSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    void BlockInput()
    {
        if (canMove && Input.GetKey(KeyCode.LeftShift))
        {
            isBlocking = true;
            animator.SetBool("Blocking", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && isBlocking)
        {
            isBlocking = false;
            animator.SetBool("Blocking", false);
        }
    }

    public void GetDamage(Damage damage)
    {
        if (isBlocking && damage.blockable)
        {
            animator.SetTrigger("BlockHit");
        }
        else
        {
            canMove = false;
            canAttack = false;
            animator.SetBool("Walking", false);
            weaponDamage = false;
            weapon.StopDamage();
            enemyAdaptation.CountAttackHit(damage.blockable);
            if (dodging != null)
            {
                StopCoroutine(dodging);
                dodging = null;
                dodgeSpeed = 0;
            }
            if (!health.SurviveDamage(damage.damage))
            {
                return;
            }
            animator.SetTrigger("Hit");
        }
        StartCoroutine(KnockingBack(damage.knockback, damage.knockbackTime));
    }

    IEnumerator KnockingBack(float speed, float time)
    {
        float currentTime = time;
        Vector3 enemyPos = new Vector3(enemy.position.x, transform.position.y, enemy.position.z);
        Vector3 moveDir = (transform.position - enemyPos);
        moveDir = new Vector3(moveDir.x, 0f, moveDir.z).normalized;
        while(currentTime > 0.05f)
        {
            transform.position = transform.position + moveDir * speed * Time.fixedDeltaTime;
            currentTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        canMove = true;
        canAttack = true;
    }

    public void Death()
    {
        animator.SetTrigger("Death");
    }

    public void DeathDone()
    {
        deathFinish?.Invoke();
    }
}
