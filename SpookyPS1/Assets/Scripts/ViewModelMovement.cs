using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelMovement : MonoBehaviour
{
    public float sway;

    private float swayX;
    private float swayY;

    private Transform startT;
    // Start is called before the first frame update
    void Start()
    {
        startT = transform;
    }

    // Update is called once per frame
    void Update()
    {
        swayX += Input.GetAxis("Mouse X") * sway * Time.deltaTime;
        swayY -= Input.GetAxis("Mouse Y") * sway * Time.deltaTime;
        swayX += sway * -swayX* Time.deltaTime* 2f;
        swayY += sway * -swayY* Time.deltaTime* 2f;
        transform.localRotation = Quaternion.Euler(startT.localRotation.x + swayY, startT.localRotation.x + swayX, startT.localRotation.z);
    }
}
