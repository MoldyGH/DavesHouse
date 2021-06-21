using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public enum MapPlaneOrientation{XZ, XY, YZ}

[Serializable]
abstract public class PhysicalMap<T> : PhysicalMap where T :PhysicalMapBehaviour{

	public T behaviour;

	override protected void FinalizeGeneration(){
		if (behaviour.enabledBatching) {
			StaticBatchingUtility.Combine (this.staticMapGo);	
			hadCombined = true;
		} else {
			hadCombined = false;
		}

		UpdateOrientation();
		
		// Additional placements, will take care of orientation
		if (behaviour.createPlayer) PlacePlayer();
        if (behaviour.createEntranceAndExit) PlaceEntranceAndExit();
	}
	
	
	protected void UpdateOrientation(){
		if (behaviour.AutomaticOrientation) return;
		
		switch(behaviour.mapPlaneOrientation){
		case MapPlaneOrientation.XY: this.rootMapGo.transform.localEulerAngles = new Vector3(-90,0,0); break;
		case MapPlaneOrientation.XZ: break;
		case MapPlaneOrientation.YZ: this.rootMapGo.transform.localEulerAngles = new Vector3(0,0,-90); break;
		}
	} 
	

	override protected void BuildRootGameObjects(){
		rootMapGo = new GameObject("Map");
		rootMapGo.transform.parent = this.behaviour.gameObject.transform;
		dynamicMapGo = new GameObject("DynamicMap");
		dynamicMapGo.transform.parent = rootMapGo.transform;
		this.dynamicMapTr = dynamicMapGo.transform;
		staticMapGo = new GameObject("StaticMap");
		staticMapGo.transform.parent = rootMapGo.transform;
		this.staticMapTr = staticMapGo.transform;	
	}
	
	private void PlacePlayer(){
        if (behaviour.createEntranceAndExit && behaviour.playerPrefab != null)
        {
			GameObject playerGo = GameObject.Instantiate(behaviour.playerPrefab) as GameObject;
			playerGo.transform.position = this.GetStartPosition();// + Vector3.up*2;
			playerGo.transform.parent = this.dynamicMapTr;
			playerGo.name = "Player";
		}
	}
	
	protected void PlaceEntranceAndExit(){
		GameObject entrancePrefab = behaviour.entrancePrefab;
		GameObject entranceGo = Instantiate(entrancePrefab,Vector3.zero,Quaternion.identity) as GameObject;
		entranceGo.transform.position = this.GetStartPosition();
		entranceGo.transform.parent = this.staticMapTr;
		entranceGo.name = "Entrance";
		
		GameObject exitPrefab = behaviour.exitPrefab;
		GameObject exitGo = Instantiate(exitPrefab,Vector3.zero,Quaternion.identity) as GameObject;
		exitGo.transform.position = this.GetEndPosition();
		exitGo.transform.parent = this.staticMapTr;
		exitGo.name = "Exit";
	}
	
	
	// Get the position of a cell starting_location in world coordinates, taking into account map orientation
	public override Vector3 GetWorldPosition(CellLocation l, int storey = 0){
		Vector3 pos = interpreter.GetWorldPosition(l,storey);

		if (behaviour.AutomaticOrientation) return pos;

//		Debug.Log (pos);
		switch(behaviour.mapPlaneOrientation){
		case MapPlaneOrientation.XY: pos = new Vector3(pos.x,pos.z,pos.y);	break;
		case MapPlaneOrientation.XZ: break; // Default
		case MapPlaneOrientation.YZ: pos = new Vector3(pos.y,-pos.x,pos.z); 	break;
		}

//		Debug.Log (pos);
		return pos;
	}



    // Get the position of a cell starting_location in world coordinates, also taking into account the size of the map
    public override Vector3 GetRealWorldPosition(CellLocation l, int storey = 0)
    {
        Vector3 pos = this.GetWorldPosition(l, storey);
        return pos;
    }
	
	
	
	
}

	
[Serializable]
abstract public class PhysicalMap : ScriptableObject{
	
	// Behaviour references, used during generation
	[SerializeField]
	protected GeneratorBehaviour generator;
	
	[SerializeField]
	protected MapInterpreter interpreter;
	
	// Dictionary used for debug
	[SerializeField]	
	public CellTypeGameObjectListDict gameObjectLists;

	// Dictionary used for zones
	[SerializeField]	
	public List<PhysicalMapZone> zones;
	
	[SerializeField]
	public GameObject rootMapGo;
	
	[SerializeField]
	public GameObject dynamicMapGo;
	
	[SerializeField]
	public GameObject staticMapGo;
	
	[SerializeField]
	public Transform staticMapTr;
	
	[SerializeField]
	public Transform dynamicMapTr;

	protected bool hadCombined = false;

	[SerializeField]
	protected VirtualMap[] virtualMaps;

	
	public void Initialise(VirtualMap[] maps, GeneratorBehaviour g, MapInterpreter i){
		virtualMaps = maps;
		generator = g;
		interpreter = i;
		
		gameObjectLists = CellTypeGameObjectListDict.Create();
		foreach(VirtualCell.CellType ct in System.Enum.GetValues(typeof(VirtualCell.CellType)).Cast<VirtualCell.CellType>()){
			gameObjectLists.Set(ct,new GameObjectList());
		}

		zones = new List<PhysicalMapZone>();
		// One zone for each storey
		for(int j=0; j<maps.Length; j++)	zones.Add(PhysicalMapZone.Create());

	}

	virtual public void CleanUp(){
		// We need to destroy newly created assets, or we'll get leaks
		DestroyImmediate (gameObjectLists);
		foreach(PhysicalMapZone z in zones) DestroyImmediate (z);
		if (hadCombined && staticMapGo.GetComponentInChildren<MeshFilter>()) {
//			Debug.Log (staticMapGo.GetComponentInChildren<MeshFilter>().sharedMesh);
			DestroyImmediate(staticMapGo.GetComponentInChildren<MeshFilter>().sharedMesh);
		}
	}
		
	abstract protected void BuildRootGameObjects();

	public void Generate(){
		if (StartGeneration()){
            if (GetShallBuildRootGameObjects()) BuildRootGameObjects();

			interpreter.Initialise(virtualMaps,this,generator);
			interpreter.ReadMaps(virtualMaps);
			interpreter.BuildMaps(virtualMaps);

			EndGeneration();
			FinalizeGeneration();
		}	
	}
	
	virtual protected void FinalizeGeneration(){
	}
	
	// May be overriden
	virtual protected bool StartGeneration(){
		return true; // Always generate
	}
	virtual protected void EndGeneration(){
	}
	
	
	abstract public Vector3 GetStartPosition();
    abstract public Vector3 GetEndPosition();
    abstract public Vector3 GetWorldPosition(CellLocation l, int storey = 0);
    abstract public Vector3 GetRealWorldPosition(CellLocation l, int storey = 0);

	protected CellLocation GetStartLocation(){
		return this.virtualMaps[0].start;
	}
	
	protected CellLocation GetEndLocation(){
		return this.virtualMaps[this.virtualMaps.Length-1].end;
	}
	
	virtual protected bool GetShallBuildRootGameObjects(){return true;}
	
	
	// TODO: Instead pass a VirtualCell to CreateObject so we can extract cell type and the orientation as well
	abstract public void CreateObject(VirtualMap map, MetricLocation l, VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation);
		
	
	public void AddToMapGameObject(VirtualCell.CellType cell_type, GameObject go){
		this.AddToMapGameObject(cell_type,go,false);
	}
	
	public void AddToMapGameObject(VirtualCell.CellType cell_type, GameObject go, bool dynamic, int zone = 0){	// TODO: extend zones, for now just used for the storeys
		if (dynamic) go.transform.parent = dynamicMapTr;
		else {
			go.transform.parent = staticMapTr;
			go.isStatic = true;
			foreach (Transform t in go.transform) {
				t.gameObject.isStatic = true;
			}
		}
		gameObjectLists.Get(cell_type).Add(go);

		// Zone handling
		zones[zone].AddObject(go);
	}
	
	
	/****************
	 * Getters
	 ****************/

    /// <summary>
    /// Returns all GameObject instances of this physical map.
    /// </summary>
    /// <returns>
    /// A List<GameObject> with all the found objects.
    /// </returns>
    public List<GameObject> GetAllObjects()
    {
        List<GameObject> list = new List<GameObject>();
        foreach(VirtualCell.CellType cell_type in this.gameObjectLists.keys){
            list.AddRange(gameObjectLists.Get(cell_type));
        }
        return list;
    }
	
	
	/// <summary>
	/// Returns the GameObject at a certain world position, or null if none is found.
	///	Note that this will return the topmost gameobject at that position, so if there is a wall above a floor, the wall will be returned.
	/// </summary>
	/// <returns>
	/// The GameObject at the position.
	/// </returns>
	/// <param name='pos'>
	/// Vector3 position.
	/// </param>
	public GameObject GetObjectAtPosition(Vector3 pos){
		RaycastHit hit;
		if (Physics.Raycast(pos+Vector3.up*50, -Vector3.up, out hit, 100)){
			return hit.transform.gameObject;
		}
		return null;
	}
	
	/// <summary>
	/// Returns all the GameObjects of a chosen SelectionObjectType.
	/// </summary>
	/// <returns>
	/// A List<GameObject> containing all objects.
	/// </returns>
	/// <param name='type'>
	/// SelectionObjectType to use.
	/// </param>
	public List<GameObject> GetObjectsOfType(SelectionObjectType type){
		List<GameObject> list = new List<GameObject>();
		foreach(VirtualCell.CellType cell_type in SelectionManager.GetCellTypes(type)){
//			Debug.Log("Getting objects for cell type " + cell_type);
			List<GameObject> new_objs = GetObjectsOfType(cell_type);
			DaedalusDebugUtils.Assert(new_objs != null, "Cannot find any list of type " + cell_type,this);
			if (new_objs != null) list.AddRange(new_objs);
		}
		return list;
	}
	
	protected List<GameObject> GetObjectsOfType(VirtualCell.CellType cell_type){
		return this.gameObjectLists.Get(cell_type);
	}
	
	public List<Vector3> GetPositionsOfType(SelectionObjectType type){
		List<GameObject> list = GetObjectsOfType(type);
		List<Vector3> positions = new List<Vector3>();
		foreach(GameObject go in list) positions.Add(go.transform.position);
		return positions;
	}

	
	/// <summary>
	/// Returns all the GameObjects of a chosen zone. (i.e.: floor)
	/// </summary>
	/// <returns>
	/// A List<GameObject> containing all objects.
	/// </returns>
	/// <param name='zone_number'>
	/// Integer representing the zone to use.
	/// </param>
	public List<GameObject> GetObjectsOfZone(int zone_number){
		return this.zones[zone_number].GetObjects();
	}


    /// <summary>
    /// Returns the dimensions of all the rooms for this map
    /// </summary>
    /// <returns>
    /// A List<VirtualRoom.Dimensions> containing the dimensions of all rooms.
    /// </returns>
    public List<VirtualRoom.Dimensions> GetAllRoomDimensions()
    {
        List<VirtualRoom.Dimensions> dims = new List<VirtualRoom.Dimensions>();
        foreach (VirtualMap map in this.virtualMaps){
            foreach (VirtualRoom.Dimensions d in map.GetRoomDimensions())
            {
                dims.Add(d);
            }
        }
        return dims;
    }


    /// <summary>
    /// Returns the dimensions of the generated dungeon.
    /// </summary>
    /// <returns>
    /// A List<VirtualMap.Dimensions> containing the dimensions of the virtual maps that compose the dungeon.
    /// </returns>
    public List<VirtualMap.Dimensions> GetMapDimensions()
    {
        List<VirtualMap.Dimensions> dims = new List<VirtualMap.Dimensions>();
        foreach (VirtualMap map in this.virtualMaps)
        {
            dims.Add(map.GetMapDimensions());
        }
        return dims;
    }
}
