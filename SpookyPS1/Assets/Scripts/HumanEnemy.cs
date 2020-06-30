using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanEnemy : MonoBehaviour
{
    [Header("Enemy radius/ranges")]
    public float lookRange = 50f; //hur långt fienden kan se
    public float hearingRadius = 30f; //hur långt fienden kan "höra"
    public float rotationSpeedMultiplier = 20f;

    private bool hearsPlayer;

    [Header("Patrol")]
    public List<Vector3> patrolRoutes; //denna verkar skapas utan "new"

    NavMeshAgent agent;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector3.Distance(target.position, transform.position); //distansen mellan spelare och fiende
        Looking(distance);
        //PatrolRoute();

        if (distance <= hearingRadius) //om spelaren är i fiendens radius
        {
            SimulateHearing();
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
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out eyes, lookRange))
        {
            Debug.Log("Player Spotted!");

            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance)
            {
                Attack();
                FaceTarget();
            }

        }
    }

    private void SimulateHearing()
    {
        Debug.Log("Enemy heard player!");
        hearsPlayer = true;
        FaceTarget();
    }


    private void Chasing() //min ide är att denna är igång i update OM fienden kan se spelaren det kommer behövas nån slags stealth state för spelaren
    {

    }

    private void Attack() //om spelaren är tillräckligt nära så kommer fieden då attackera
    {

    }


    private void PatrolRoute()
    {
        int i = 0;
        agent.SetDestination(patrolRoutes[i]);
        Debug.Log(patrolRoutes[i]);
        if (agent.transform.position == patrolRoutes[i])
        {

        }
    }

}