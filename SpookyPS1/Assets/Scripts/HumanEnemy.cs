using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanEnemy : MonoBehaviour
{
    [Header("Enemy radius/ranges")]
    public float lookRange; //hur långt fienden kan se
    public float hearingRadius; //hur långt fienden kan "höra"
    public float enemyDangerZone; //om spelaren är så här nära så blir man konsant jagad
    public float rotationSpeedMultiplier;

    private bool patrolAssigned;

    [Header("Patrol")]
    public List<Transform> patrolRoutes; //denna verkar skapas utan "new"
    public GameObject patrolPrefab;
    private int currentPatrolPos;
    private bool reachedPatrolPos;

    [Header("Search")]
    public bool isSearching;
    public bool playerSpotted;
    public bool chasingPlayer;

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

        if (distance <= hearingRadius) //om spelaren är i fiendens hörsel radius
        {
            SimulateHearing();
        }

        if (distance <= enemyDangerZone) //om spelaren är för nära fienden 
        {
            Debug.Log("Player has entered an enemies danger zone!");
            chasingPlayer = true;
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

            if (eyes.collider.tag.Equals("Player") || chasingPlayer)
            {
                Chasing(distance);
                isSearching = false;
                Debug.Log("Player Spotted!");
                playerSpotted = true;
            }

            if(!eyes.collider.tag.Equals("Player") || (!chasingPlayer))
            {
                if (!isSearching)
                {
                    playerSpotted = false;
                    Searching();
                }
            }
        }
    }

    private void Searching()
    {

        isSearching = true;

        if (isSearching)
        {
            StartCoroutine(SearchCoroutine());
        }
    }


    private IEnumerator SearchCoroutine()
    {
        yield return new WaitForSeconds(2f);
        agent.SetDestination(VectorSearchRandomiser());

        if (isSearching)
        {
            Searching();
        }
    }

    private IEnumerator EndingChase()
    {
        yield return new WaitForSeconds(5f);

        if (!playerSpotted)
        {
            chasingPlayer = false;
        }
    }

    private Vector3 VectorSearchRandomiser()
    {
        float minRandom = -25f;
        float maxRandom = 25f;

        return new Vector3(this.gameObject.transform.position.x +
                Random.Range(minRandom, maxRandom),
                this.gameObject.transform.position.y,
                this.gameObject.transform.position.z +
                Random.Range(minRandom, maxRandom));
    }

    private void PatrolRoute() //OBS denna anvämds ej
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
            }
        }
    }

    private void SimulateHearing()
    {
        Debug.Log("Enemy heard player!");
        FaceTarget();
    }

    private void Chasing(float distance) //min ide är att denna är igång i update OM fienden kan se spelaren det kommer behövas nån slags stealth state för spelaren
    {
        chasingPlayer = true;
        Debug.Log("Enemy is chasing!");
        agent.SetDestination(target.position);

        FaceTarget();

        if (distance <= agent.stoppingDistance)
        {
            Debug.Log("Enemy can attack!");
            Attack();
            FaceTarget();
        }

        if (!playerSpotted)
        {
            StartCoroutine(EndingChase());
        }
    }

    private void Attack() //om spelaren är tillräckligt nära så kommer fieden då attackera
    {
        Debug.Log("Enemy attacks!");
    }


    /* Problem
     * Fienden kan inte gå till specifika patrull värden eftersom att fienden Y värde blir något specifkt långt tal, man måste skriva in exakt y värde
     * Ta bort y värden och bara kolla på x och z värden? eller tänk om hela patrull lösningen?
     * Om spelaren hörs, så bör det skapas en zon likt hearing zonen som man måste gå bort från för att fly från fienden
     * Man skulle kunna randomisa positions värden nära spelaren för att få fienden att leta, om man blir hittad av en fiende. Kanske om man förbestämmer flera positioner innan och sedan så väljer fienden de som är närmast?
     * fienden bör jaga även fast den inte direkt ser spelaren, detta är inte implementerat
    */
}