using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementPlayer : MovementChar
{
    [HideInInspector]
    public UnityEvent attack;

    Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (canMove)
        {
            MovementInput();
        }
    }

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetHit(new Damage { blockable = true, damage = 10, knockback = 60f, knockbackTime = 1f }, transform.position + Vector3.back);
        }
    }

    void RotateCharacter(Vector3 direction)
    {
        float angle = Mathf.Atan(direction.x / direction.z) * Mathf.Rad2Deg;
        if (direction.z < 0f)
            angle += 180;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public override void StartJump()
    {
        coroutines[jumping] = Jumping(KeyboardDirection());
        StartCoroutine(coroutines[jumping]);
    }

    Vector3 KeyboardDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector3(horizontal, 0f, vertical).normalized;
    }

    public override void AttackStart(int attackSq)
    {
        base.AttackStart(attackSq);
        attack?.Invoke();
        if (weapon.weaponType == WeaponType.Bow)
        {
            FaceMouse();
        }
    }

    protected virtual void FaceMouse()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
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
