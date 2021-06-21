using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public MeshCollider trigger;
    
    public Material open;
    
    public Material closed;
    
    public AudioSource doorAudio;
    
    public AudioClip doorOpen;
    
    private float openTime;
    
    private bool doorOpened;
    
    public MeshRenderer inside;
    
    public MeshRenderer outside;
    // Start is called before the first frame update
    void Start()
    {
        this.doorAudio.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.openTime > 0f)
	  {
	  	this.openTime -= 1f * Time.deltaTime;
	  }
	  if (this.openTime <= 0f & this.doorOpened)
	  {
	  	this.doorOpened = false;
		this.inside.material = this.closed;
		this.outside.material = this.closed;
	  }
    }
    public void OnTriggerEnter(Collider other)
    {
    	this.doorAudio.PlayOneShot(this.doorOpen, 1f);
    }
    public void OnTriggerStay(Collider other)
    {
    	this.doorOpened = true;
		this.inside.material = this.open;
		this.outside.material = this.open;
		this.openTime = 2f;
    }
}
