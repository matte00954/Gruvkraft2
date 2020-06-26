using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanEnemy : MonoBehaviour
{

    public float lookRadius = 10f; //hur långt fienden kan se

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

        if(distance <= lookRadius) //om fienden ser spelaren
        {
            agent.SetDestination(target.position); 


            if(distance <= agent.stoppingDistance)
            {
                //attack target
                FaceTarget();
            }
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized; 
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        //big brain maths (har absolut inte tagit detta från från en tutorial), gör så att fienden roterar mot spelaren korrekt 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        //Dessa gör så att man ser en röd cirkel runt fienden i unity
    }
}
