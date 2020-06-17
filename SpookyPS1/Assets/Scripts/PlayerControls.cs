using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Maximum walkspeed")]
    public float moveSpeed;
    [Header("Walking acceleration")]
    public float acc;
    [Header("Horisontal and vertical look sensitivity")]
    public float vSens;
    public float hSens;
    public GameObject camera;

    private Rigidbody rb;

    private float walkDir;

    private int xDir;
    private int yDir;

    private bool canMove;
    private bool grounded;
    private bool accelerating;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grounded = false;
    }

    void Update()
    {
        //tillfällig if sats
        if (true)
        {
            Look();
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && grounded)
            {
                //accelerating sätts här tillfälligt
                accelerating = true;
                Move();
                LimitVelocity();
            }
            else
            {
                accelerating = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) // Press F to pay respects
        {

        }

        if (!accelerating && grounded)
        {
            SlowDown();
        }
        Debug.Log(grounded);

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
        rb.AddForce(transform.TransformDirection(new Vector3(Mathf.Cos(Mathf.Deg2Rad * walkDir), 0, Mathf.Sin(Mathf.Deg2Rad * walkDir))) * acc);
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
        if (rb.velocity.magnitude > moveSpeed)
        {
            if (rb.velocity.magnitude * 0.8f * Time.deltaTime > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * rb.velocity.magnitude * 0.8f * Time.deltaTime;
            }
            else
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
    }

    private void SlowDown()
    {
        if (rb.velocity.magnitude - acc * Time.deltaTime> 0)
        {
            rb.velocity = rb.velocity.normalized * (rb.velocity.magnitude - acc * Time.deltaTime);
        }
        else
        {
            rb.velocity = rb.velocity.normalized * 0;

        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            grounded = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") == true)
        {
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") == true)
        {
            grounded = false;
        }
    }
}
