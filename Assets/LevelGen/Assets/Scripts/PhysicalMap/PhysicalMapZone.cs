using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// A Zone in a physical map, which holds all objects coming from a single asset pack
[Serializable]
public class PhysicalMapZone : ScriptableObject {
	[SerializeField]
	public GameObjectListContainer objects;

//	public List<GameObject> objects;
	// TODO: public AssetPack pack;

	public static PhysicalMapZone Create() {
		PhysicalMapZone zone = ScriptableObject.CreateInstance<PhysicalMapZone>();
//		zone.objects = new List<GameObject>();
		zone.objects = new GameObjectListContainer();
		return zone;
	}
	
	public void AddObject(GameObject go){
		objects.Add(go);
	}

	public List<GameObject> GetObjects(){
		return objects._inner_list;
	}
}