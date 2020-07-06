using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanEnemy : MonoBehaviour
{
    [Header("Enemy radius/ranges")]
    public float lookRange; //hur långt fienden kan se
    public float hearingRadius; //hur långt fienden kan "höra"
    public float rotationSpeedMultiplier;

    //private bool hearsPlayer;
    private bool seesPlayer;
    private bool patrolAssigned;

    [Header("Patrol")]
    public List<Transform> patrolRoutes; //denna verkar skapas utan "new"
    public GameObject patrolPrefab;
    private int currentPatrolPos;
    private bool reachedPatrolPos;

    NavMeshAgent agent;
    Transform target;
    Rigidbody rb;

    // Bit shift the index of the layer (8) to get a bit mask
    private int layerMask = 1 << 8;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();

        for (int i = 0; i < patrolRoutes.Count; i++)
        {
            Instantiate(patrolPrefab, patrolRoutes[i]); //skapar spelobjekt för checkpoints
        }

    }

    // Update is called once per frame
    void Update()
    {

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        // https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
        layerMask = ~layerMask;
        float distance = Vector3.Distance(target.position, transform.position); //distansen mellan spelare och fiende
        Looking(distance);

        if (distance <= hearingRadius) //om spelaren är i fiendens radius
        {
            SimulateHearing();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(agent.transform.position);
            Debug.Log(agent.destination);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("patrolCheckpoint"))
        {

        }
    }


    private void LateUpdate()
    {
        PatrolRoute(); //tveksamt om jag vill ha detta i en update metod
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeedMultiplier);
        //big brain maths (har absolut inte tagit detta från från en tutorial), gör så att fienden roterar mot spelaren korrekt 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
        //Dessa gör så att man ser en röd cirkel runt fienden i unity
    }

    private void Looking(float distance)
    {
        RaycastHit eyes;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out eyes, lookRange, layerMask))
        {

            if (eyes.collider.tag.Equals("Player"))
            {
                seesPlayer = true;

                Debug.Log("Player Spotted!");

                agent.SetDestination(target.position);

            }

            if (distance <= agent.stoppingDistance)
            {
                Attack();
                FaceTarget();
            }
            else
            {
                seesPlayer = false;
            }
        }
    }

    private void PatrolRoute()
    {
        if (reachedPatrolPos) //agent reach patrol pos
        {

            Debug.Log("Agent reached patrol position");
            if (currentPatrolPos >= patrolRoutes.Count)
            {
                currentPatrolPos = 0;
            }

            else
            {
                currentPatrolPos++;
            }

            patrolAssigned = false;

        }
        else
        {
            if (!patrolAssigned)
            {
                patrolAssigned = true;
                Debug.Log("Patrol position assigned");
                agent.SetDestination(patrolRoutes[currentPatrolPos].gameObject.transform.position);
            }
        }
    }

    private void SimulateHearing()
    {
        Debug.Log("Enemy heard player!");
        FaceTarget();
    }

    private void Chasing() //min ide är att denna är igång i update OM fienden kan se spelaren det kommer behövas nån slags stealth state för spelaren
    {

    }

    private void Attack() //om spelaren är tillräckligt nära så kommer fieden då attackera
    {

    }


    /* Problem
     * Fienden kan inte gå till specifika patrull värden eftersom att fienden Y värde blir något specifkt långt tal, man måste skriva in exakt y värde
     * Ta bort y värden och bara kolla på x och z värden? eller tänk om hela patrull lösningen?
     * Om spelaren hörs, så bör det skapas en zon likt hearing zonen som man måste gå bort från för att fly från fienden
     * Man skulle kunna randomisa positions värden nära spelaren för att få fienden att leta, om man blir hittad av en fiende. Kanske om man förbestämmer flera positioner innan och sedan så väljer fienden de som är närmast?
    */
}