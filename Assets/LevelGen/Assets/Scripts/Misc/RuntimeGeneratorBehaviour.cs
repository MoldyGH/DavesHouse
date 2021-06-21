using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RegenerateDungeonTriggerBehaviour : MonoBehaviour {
	public RuntimeGeneratorBehaviour runtimeGenerator;
	
	public void OnTriggerEnter(Collider other){
		runtimeGenerator.GenerateNewMap();	
	}
}

public class RuntimeGeneratorBehaviour : MonoBehaviour
{
	private GeneratorBehaviour generator;
	
	public void Start(){
		this.generator = this.gameObject.GetComponent<GeneratorBehaviour>();
	
		GenerateNewMap();
		
	}
	
	public void GenerateNewMap(){
		
		// Generate, will then send the DungeonGenerated message to this gameobject
		this.generator.Generate();
		
	}
	
	public void DungeonGenerated(){
		Debug.Log("Dungeon has been generated!");
		
		// Place the trigger for regeneration at the exit
		PhysicalMap physicalMap = this.generator.GetPhysicalMap();

		// Get the end position
		Vector3 endPos = physicalMap.GetEndPosition();
		
		// Create and place the exit trigger
		GameObject exitTrigger = new GameObject("ExitTrigger");
		exitTrigger.transform.parent = physicalMap.dynamicMapTr;
		exitTrigger.transform.position = endPos;
		
		// Add a box trigger to it
		BoxCollider coll = exitTrigger.AddComponent<BoxCollider>();
		coll.size = new Vector3(5,5,5);
		coll.isTrigger = true;
		
		// Make it regenerate the map
		exitTrigger.AddComponent<RegenerateDungeonTriggerBehaviour>().runtimeGenerator = this;
	}
	
	
}
	
