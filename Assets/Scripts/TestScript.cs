using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    IEnumerator countNumber;
    bool clicked = false;

    void Start()
    {
        countNumber = CountNumber();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!clicked)
            {
                clicked = true;
                StartCoroutine(countNumber);
            }
            else
            {
                clicked = false;
                StopCoroutine(countNumber);
            }
        }
    }

    IEnumerator CountNumber()
    {
        int number = 0;
        while (true)
        {
            Debug.Log(number);
            number++;
            yield return new WaitForSeconds(1f);
        }
    }
}
