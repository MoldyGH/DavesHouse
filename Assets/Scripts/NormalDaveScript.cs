using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDaveScript : MonoBehaviour
{
    public AudioSource normalDaveAudio;
    
    public AudioClip daveHi;
    
    public Sprite DaveTalk;
    
    public Sprite NormalDave;
    
    public GameObject DaveSpriteObject;
    
    public AudioClip prize;
    
    public GameController gc;
    // Start is called before the first frame update
    void Start()
    {
        normalDaveAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!normalDaveAudio.isPlaying)
	  {
	  	this.DaveSpriteObject.GetComponent<SpriteRenderer>().sprite = this.NormalDave;
	  }
	  else
	  {
	  	this.DaveSpriteObject.GetComponent<SpriteRenderer>().sprite = this.DaveTalk;
	  }
	  
    }
}
