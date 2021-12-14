using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttackParticle : MonoBehaviour
{
    // Class for controlling the heavy attack particle

    // Variables
    [HideInInspector]
    public MeleeWeapon weapon;
    bool damaging;
    string targetTag;
    float colliderZ;
    public float particleTime;
    public float colliderSpeed;
    public SphereCollider sphereCollider;

    // When the object get enabled
    private void OnEnable()
    {
        damaging = true;
        sphereCollider.center = new Vector3(0f, 0f, colliderZ);
        sphereCollider.enabled = true;
        StartCoroutine(MovingCollider());
    }

    private void Start()
    {
        colliderZ = sphereCollider.center.z;
    }

    // Coroutine to move the collider
    IEnumerator MovingCollider()
    {
        float time = particleTime;
        while (time > 0f)
        {
            sphereCollider.center = new Vector3(sphereCollider.center.x + colliderSpeed * Time.fixedDeltaTime, 0f, colliderZ);
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        sphereCollider.enabled = false;
    }

    // Method to assign the target for the particle
    public void AssignParticle(MeleeWeapon weapon)
    {
        this.weapon = weapon;
        targetTag = weapon.targetTag;
    }

    // When the particle hit the target
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
