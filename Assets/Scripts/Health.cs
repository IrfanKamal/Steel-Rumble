using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int health;
    public AudioSource hitSound, deathSound;
    public UnityEvent<int> healthChange;
    public UnityEvent death;

    [HideInInspector]
    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
    }

    public bool SurviveDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        healthChange?.Invoke(currentHealth);
        if (currentHealth > 0)
        {
            //hitSound.Play();
            return true;
        }
        else
        {
            death?.Invoke();
            //deathSound.Play();
            return false;
        }
    }

}
