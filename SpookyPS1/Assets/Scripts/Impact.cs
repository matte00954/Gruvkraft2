using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    public ParticleSystem impact;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Mark"&& col.gameObject.tag == "Enemy" == false)
        {
            Debug.Log("PEW");
            Instantiate(impact, transform.position, Quaternion.LookRotation(col.contacts[0].normal));
            impact.Play();
            Destroy(this.gameObject);
        }
    }

}
