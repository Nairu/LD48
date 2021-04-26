using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPickup : MonoBehaviour
{
    [SerializeField]
    private GameObject book;
    [SerializeField]
    private Weapon weaponInfo;
    [SerializeField]
    private float bobSpeed;
    [SerializeField]
    private float rotSpeed;
    [SerializeField]
    private float bobHeight;
    [SerializeField]
    private float offset;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = book.transform.position;
        pos.y = offset + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        Vector3 rot = book.transform.rotation.eulerAngles;
        rot.y += Time.deltaTime * rotSpeed;
        book.transform.position = pos;
        book.transform.rotation = Quaternion.Euler(rot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<FirstPersonController>().AddWeapon(weaponInfo);
            Destroy(gameObject);
        }
    }
}
