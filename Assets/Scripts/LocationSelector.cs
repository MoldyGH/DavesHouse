using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LocationSelector : MonoBehaviour
{
    public Transform wanderTarget;
    // Start is called before the first frame update
    void Start()
    {
        wanderTarget = base.transform;
        base.transform.position = new Vector3(UnityEngine.Random.Range(161f, 282f), base.transform.position.y, UnityEngine.Random.Range(44f, -265f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WanderAgent")
        {
            base.transform.position = new Vector3(UnityEngine.Random.Range(161f, 282f), base.transform.position.y, UnityEngine.Random.Range(44f, -265f));
        }
        other.GetComponent<NavMeshAgent>().SetDestination(wanderTarget.position);
    }
}
