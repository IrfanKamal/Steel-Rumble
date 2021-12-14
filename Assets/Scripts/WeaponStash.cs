using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStash : MonoBehaviour
{
    // Class for object to store weapons

    // Variables
    public List<WeaponBase> stashedWeapon;
    public GameObject buttonIcon;
    public Transform weaponLocation;
    public float distance;
    bool assigned = true;

    // Start is called before the first frame update
    void Start()
    {
        if (stashedWeapon[0].user == null)
        {
            ReturnWeapon();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && buttonIcon.activeSelf == true)
        {
            AssignWeapon();
        }
    }

    // Method to assign the weapon stashed to player
    void AssignWeapon()
    {
        assigned = true;
        buttonIcon.SetActive(false);
        MovementPlayer player = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementPlayer>();
        WeaponBase currentWeapon = player.weapon;
        foreach(GameObject stash in GameObject.FindGameObjectsWithTag("WeaponStash"))
        {
            WeaponStash stashScript = stash.GetComponent<WeaponStash>();
            if(stashScript.stashedWeapon[0] == currentWeapon)
            {
                stashScript.ReturnWeapon();
                break;
            }
        }
        foreach(WeaponBase weapon in stashedWeapon)
        {
            weapon.AssignWeapon(player);
        }
    }

    // Method to return the weapon
    public void ReturnWeapon()
    {
        assigned = false;
        for (int i = 0; i < stashedWeapon.Count; i++)
        {
            stashedWeapon[i].DeassignWeapon();
            stashedWeapon[i].transform.position = new Vector3(weaponLocation.position.x, weaponLocation.position.y, weaponLocation.position.z + distance * i);
            stashedWeapon[i].transform.rotation = weaponLocation.rotation;
            stashedWeapon[i].transform.localScale = weaponLocation.localScale;
        }
    }

    // When player get close
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !assigned)
        {
            buttonIcon.SetActive(true);
        }
    }

    // When player leave
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !assigned)
        {
            buttonIcon.SetActive(false);
        }
    }
}
