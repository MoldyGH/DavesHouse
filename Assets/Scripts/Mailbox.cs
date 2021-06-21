
using UnityEngine;

public class Mailbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!base.GetComponent<AudioSource>().isPlaying)
            {
                base.GetComponent<AudioSource>().Play();
            }
        }       
    }
}
