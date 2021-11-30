using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public AudioSource hitSound, deathSound;


    int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
    }

    public bool SurviveDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
        {
            hitSound.Play();
            return true;
        }
        else
        {
            deathSound.Play();
            return false;
        }
    }

}
