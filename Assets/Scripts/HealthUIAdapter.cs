using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthUIAdapter : MonoBehaviour
{
    public Health charHealth;
    Slider healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider = GetComponent<Slider>();
        healthSlider.maxValue = charHealth.health;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = charHealth.currentHealth;
    }
}
