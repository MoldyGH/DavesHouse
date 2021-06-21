using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : MonoBehaviour
{
    public NavMeshAgent agent;
    public int wanderTargetSelector;
    public Transform one;
    public Transform two;
    public Transform three;
    public Transform four;
    // Start is called before the first frame update
    void Start()
    {
        this.agent = base.GetComponent<NavMeshAgent>();
	  this.wanderTargetSelector = UnityEngine.Random.Range(0, 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.wanderTargetSelector == 1)
	  {
	  	this.agent.SetDestination(this.one.position);
	  }
	  if (this.wanderTargetSelector == 2)
	  {
	  	this.agent.SetDestination(this.two.position);
	  }
	  if (this.wanderTargetSelector == 3)
	  {
	  	this.agent.SetDestination(this.three.position);
	  }
	  if (this.wanderTargetSelector == 4)
	  {
	  	this.agent.SetDestination(this.four.position);
	  }
    }
}
