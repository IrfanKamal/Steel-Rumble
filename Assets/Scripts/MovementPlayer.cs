using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementPlayer : MovementChar
{
    // Class for character that can be controlled by player

    // Event for enemy adaptation
    [HideInInspector]
    public UnityEvent<int> attack;
    [HideInInspector]
    public UnityEvent<bool> getDamage;

    Camera cameraMain;

    protected override void Start()
    {
        base.Start();
        cameraMain = Camera.main;
    }

    private void Update()
    {
        if (canMove)
        {
            MovementInput();
        }
    }

    // Control player input
    void MovementInput()
    {
        Vector3 kbDir = KeyboardDirection();
        if (interupable)
        {
            if (kbDir != Vector3.zero)
            {
                RotateCharacter(kbDir);
                StartWalk();
            }
        }
        if (kbDir == Vector3.zero && animator.GetBool(walkingPar))
        {
            StopWalk();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetMouseButtonDown(0))
        {
            LightAttack();
        }
        if (Input.GetMouseButtonDown(1))
        {
            HeavyAttack();
        }
        if (Input.GetKey(KeyCode.LeftShift) && !animator.GetBool(blockingPar))
        {
            StartBlock();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopBlock();
        }
        /*if (Input.GetKeyDown(KeyCode.P))
        {
            GetHit(new Damage { blockable = true, damage = 10, knockback = 60f, knockbackTime = 1f }, transform.position + Vector3.back);
        }*/
    }

    // To Rotate character
    void RotateCharacter(Vector3 direction)
    {
        float angle = Mathf.Atan(direction.x / direction.z) * Mathf.Rad2Deg;
        if (direction.z < 0f)
            angle += 180;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    // Initiate start jumping
    public override void StartJump()
    {
        coroutines[jumping] = Jumping(KeyboardDirection());
        StartCoroutine(coroutines[jumping]);
    }

    // To see where the keyboard direction
    Vector3 KeyboardDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector3(horizontal, 0f, vertical).normalized;
    }

    // Starting the attack
    public override void AttackStart(int attackSq)
    {
        base.AttackStart(attackSq);
        attack?.Invoke(attackSq);
        if (weapon.weaponType == WeaponType.Bow)
        {
            FaceMouse();
        }
    }

    // when get damage
    public override void GetDamage(Damage damage, Vector3 knockDir)
    {
        base.GetDamage(damage, knockDir);
        getDamage?.Invoke(damage.blockable);
    }

    // Rotate position toward mouse when using bow
    protected virtual void FaceMouse()
    {
        Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 mousePos = hit.point;
            mousePos.y = transform.position.y;
            Vector3 dir = mousePos - transform.position;
            float angle = Mathf.Atan(dir.x / dir.z) * Mathf.Rad2Deg;
            if (dir.z < 0f)
                angle += 180;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}
