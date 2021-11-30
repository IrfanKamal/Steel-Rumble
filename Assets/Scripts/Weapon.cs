using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public string target;
    public Damage currentDamage;
    public bool damaging = false;
    public UnityEvent<Damage> onHit;
    public bool alreadyHit = false;
    public AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDamage(Damage damage)
    {
        currentDamage = damage;
        damaging = true;

    }

    public void StopDamage()
    {
        damaging = false;
        alreadyHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damaging && other.CompareTag(target) && !alreadyHit)
        {
            alreadyHit = true;
            hitSound.Play();
            onHit?.Invoke(currentDamage);
        }
    }
}
