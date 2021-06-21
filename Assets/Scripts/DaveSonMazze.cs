using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class DaveSonMazze : MonoBehaviour
{
    public NavMeshAgent daveSon;

    public Transform player;

    public Collider playerCol;

    public GameObject jumpscareImage;

    public AudioSource jumpscareSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.daveSon.SetDestination(this.player.position);
    }
    private void OnTriggerEnter(Collider play)
    {
        if (play = playerCol)
        {
            Jumpscare();
        }
    }
    public void Jumpscare()
    {
        jumpscareImage.SetActive(true);
        jumpscareSound.Play();
        if (!jumpscareSound.isPlaying)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
