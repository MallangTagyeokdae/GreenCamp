using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    Transform cam;
    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (GameManager.instance.clickedObject[0].GetComponent<Unit>() != gameObject.transform.parent?.GetComponent<Unit>()) gameObject.SetActive(false);
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
    }

    public void UpdateHealthBar(Unit unit)
    {
        unit.transform.Find("HealthBar").GetComponent<Canvas>().gameObject.SetActive(true);
        Slider healthBar = unit.transform.Find("HealthBar/UnitCurrentHealth").GetComponent<Slider>();
        healthBar.value = (float)(unit.unitCurrentHealth * 1.0 / unit.unitMaxHealth);
    }
}
