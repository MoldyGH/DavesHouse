using UnityEngine;
using System.Collections;

public abstract class AbstractOpenDoors : MonoBehaviour {

	private int inside_objects_number = 0;

//	void Update(){
//		if(Input.GetKeyDown("a")) AnimateOpen ();
//		if(Input.GetKeyDown("s")) AnimateClose ();
//	}

	
	void OnTriggerEnter(Collider other){
		inside_objects_number++;
		if (inside_objects_number == 1) {
			AnimateOpen();
		}
	}

	void OnTriggerExit(Collider other){
		inside_objects_number--;
		if (inside_objects_number == 0) {
			AnimateClose();
		} 
	}

	
	abstract protected void AnimateOpen();
	abstract protected void AnimateClose();

}
