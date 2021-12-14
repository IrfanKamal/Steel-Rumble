using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Class for controlling the arrow

    // Variables
    public float moveSpeed;
    public float flyTime;
    public ObjectPool heavyAttackParticlePool;
    string targetTag;

    [HideInInspector]
    public bool moving = false;
    [HideInInspector]
    public bool heavyAttack = false;
    [HideInInspector]
    public BowWeapon weapon;
    Vector3 moveDir;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject objectPool in GameObject.FindGameObjectsWithTag("ObjectPool"))
        {
            ObjectPool script = objectPool.GetComponent<ObjectPool>();
            if (script.objectName == "ArrowParticle")
            {
                heavyAttackParticlePool = script;
                break;
            }
        }
    }

    // Decide where to move
    public void DetermineMoveDir()
    {
        float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        float zCor = Mathf.Cos(angle);
        float xCor = Mathf.Sin(angle);
        moveDir = new Vector3(xCor, 0f, zCor).normalized;
        //Debug.Log(transform.rotation.eulerAngles);
        targetTag = weapon.targetTag;
        StartCoroutine(Flying());
    }

    // Coroutine for start moving
    IEnumerator Flying()
    {
        rb.velocity = moveDir * moveSpeed;
        yield return new WaitForSeconds(flyTime);
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    // When hit the target
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            MovementChar target = other.GetComponent<MovementChar>();
            weapon.DealDamage(target, weapon.user.transform.position);
            if (heavyAttack)
            {
                GameObject heavyAttackParticle = heavyAttackParticlePool.RequestObject();
                heavyAttackParticle.transform.position = transform.position;
                heavyAttackParticle.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }
}
