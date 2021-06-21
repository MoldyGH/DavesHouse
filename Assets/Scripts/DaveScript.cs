using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DaveScript : MonoBehaviour
{
    public NavMeshAgent dave;
    
    public Transform player;
    
    public AudioSource daveAudio;

    public SpriteRenderer daveSprite;

    public Sprite davePied;
    // Start is called before the first frame update
    void Start()
    {
        this.dave.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        dave.SetDestination(player.position);
    }
}
