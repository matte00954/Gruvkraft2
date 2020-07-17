using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public GameObject camera;
    public GameObject impact;

    [Header("Maximum walkspeed")]
    public float moveSpeed;

    [Header("Walking acceleration")]
    public float acc;

    [Header("Jumpforce")]
    public float jumpForce;

    [Header("Horisontal and vertical look sensitivity")]
    public float vSens;
    public float hSens;

    [Header("Raycast variables")]
    public float maxObjectInteractionDistance;
    public LayerMask objectMask;
    //layerMask som man kan lägga på objekt som man kan interagera med

    [Header("Shooting Variables")]
    public float range;

    private Rigidbody rb;

    private float walkDir;

    private int xDir;
    private int yDir;

    private bool canMove;
    private bool grounded;
    private bool accelerating;

    private Animator anim;
    private Animator anim2;

    void Start()
    {
        anim = GameObject.Find("Player/Camera/Camera/ViewModel/spooky1911anim 1").GetComponent<Animator>();
        anim2 = GameObject.Find("Player/Camera/Camera/ViewModel").GetComponent<Animator>();
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
                anim2.SetBool("Walking", true);
            }
            else
            {
                accelerating = false;
                anim2.SetBool("Walking", false);
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.F)) // Press F to pay respects
        {
            ObjectInteraction();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) // Skjut
        {
            Shot();
        }

        if (!accelerating && grounded)
        {
            SlowDown();
        }
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

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
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

    private void ObjectInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxObjectInteractionDistance /*objectMask*/)) //OBS objectMask ska användas senare!!!
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }

    private void Shot()
    {
        anim.Play("Fire");
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range))
        {
            Debug.DrawRay(camera.transform.position, camera.transform.forward * hit.distance, Color.yellow);
            GameObject dust;
            dust= Instantiate(impact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            dust.GetComponent<ParticleSystem>().Play();
        }
    }
}
