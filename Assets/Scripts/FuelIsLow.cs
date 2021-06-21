using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FuelIsLow : MonoBehaviour
{
    public NavMeshAgent agent;

    public AudioSource audioDevice;

    public Transform player;

    public LocationSelector wanderer;
    // Start is called before the first frame update
    void Start()
    {
        agent = base.GetComponent<NavMeshAgent>();
        audioDevice = base.GetComponent<AudioSource>();
        this.Wander();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TargetPlayer()
    {
        agent.SetDestination(player.position);
    }
    public void Wander()
    {
        agent.SetDestination(wanderer.wanderTarget.position);
    }
}
