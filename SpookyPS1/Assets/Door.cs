using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject door;
    public bool open;
    public float targetAngle;
    private float angle;
    private Transform original;

    void Start()
    {
        original = transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(original.transform.rotation.x, original.transform.rotation.y+ angle, original.transform.rotation.z);
        if (angle< targetAngle)
        {
            angle += Time.deltaTime* 10;
            if (angle> targetAngle)
            {
                angle = targetAngle;
            }
        }
        if (angle > targetAngle)
        {
            angle -= Time.deltaTime * 10;
            if (angle < targetAngle)
            {
                angle = targetAngle;
            }
        }
    }

    void toggleOpen()
    {
        if (!open)
        {
            targetAngle = 0;
        }
    }
}
