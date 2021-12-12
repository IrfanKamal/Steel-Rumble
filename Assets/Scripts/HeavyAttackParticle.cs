using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttackParticle : MonoBehaviour
{
    [HideInInspector]
    public MeleeWeapon weapon;
    bool damaging;
    string targetTag;
    float colliderZ;
    public float particleTime;
    public float colliderSpeed;
    public SphereCollider collider;

    private void OnEnable()
    {
        damaging = true;
        collider.center = new Vector3(0f, 0f, colliderZ);
        collider.enabled = true;
        StartCoroutine(MovingCollider());
    }

    private void Start()
    {
        colliderZ = collider.center.z;
    }

    IEnumerator MovingCollider()
    {
        float time = particleTime;
        while (time > 0f)
        {
            collider.center = new Vector3(collider.center.x + colliderSpeed * Time.fixedDeltaTime, 0f, colliderZ);
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        collider.enabled = false;
    }
    public void AssignParticle(MeleeWeapon weapon)
    {
        this.weapon = weapon;
        targetTag = weapon.targetTag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && damaging)
        {
            damaging = false;
            MovementChar target = other.GetComponent<MovementChar>();
            weapon.DealDamage(target, transform.position);
        }
    }
}
