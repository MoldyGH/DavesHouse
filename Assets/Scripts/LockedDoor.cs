using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockedDoor : MonoBehaviour
{
    public Material normalDoor;
    
    public Material lockedDoor;
    
    public GameController gc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gc.supplies == 1)
	  {
	  	base.GetComponent<MeshRenderer>().material = this.normalDoor;
		base.GetComponent<MeshCollider>().enabled = false;
	  }
       
    }
}
