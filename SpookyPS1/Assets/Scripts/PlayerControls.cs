using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float moveSpeed;
    public float acc;
    public float vSens;
    public float hSens;
    public GameObject camera;

    private float walkDir;
    private int xDir;
    private int yDir;
    private bool canMove;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Look();
        if (canMove)
        {
            Move();
        }
        else
        {
            SlowDown();
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }
        LimitVelocity();
    }
    private void Look()
    {
        camera.transform.Rotate(-Input.GetAxis("Mouse Y")* Time.deltaTime* vSens,0,0);
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * hSens, 0);
    }

    private void Move()
    {
        xDir = 0;
        yDir = 0;
        if (Input.GetKey(KeyCode.W))
        {
            yDir++;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xDir--;
        }
        if (Input.GetKey(KeyCode.S))
        {
            yDir--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            xDir++;
        }
        GetDir();
        GetComponent<Rigidbody>().AddForce(transform.TransformDirection(new Vector3(Mathf.Cos(Mathf.Deg2Rad * walkDir), 0, Mathf.Sin(Mathf.Deg2Rad * walkDir))) * acc);
    }

    private void GetDir()
    {
        if (xDir == 1 && yDir == 0)
        {
            walkDir = 0;
        }
        if (xDir == 1 && yDir == 1)
        {
            walkDir = 45;
        }
        if (xDir == 0 && yDir == 1)
        {
            walkDir = 90;
        }
        if (xDir == -1 && yDir == 1)
        {
            walkDir = 135;
        }
        if (xDir == -1 && yDir == 0)
        {
            walkDir = 180;
        }
        if (xDir == -1 && yDir == -1)
        {
            walkDir = 225;
        }
        if (xDir == 0 && yDir == -1)
        {
            walkDir = 270;
        }
        if (xDir == 1 && yDir == -1)
        {
            walkDir = 315;
        }
    }

    private void LimitVelocity()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude > moveSpeed)
        {
            if (GetComponent<Rigidbody>().velocity.magnitude * 0.8f * Time.deltaTime > moveSpeed)
            {
                GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * GetComponent<Rigidbody>().velocity.magnitude * 0.8f * Time.deltaTime;
            }
            else
            {
                GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * moveSpeed;
            }
        }
    }

    private void SlowDown()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude - acc * Time.deltaTime> 0)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * (GetComponent<Rigidbody>().velocity.magnitude - acc * Time.deltaTime);
        }
        else
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * 0;

        }

    }
}
