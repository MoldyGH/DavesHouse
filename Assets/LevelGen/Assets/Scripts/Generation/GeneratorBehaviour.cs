using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum MapDimensionsType{TWO_DEE, THREE_DEE}

public class GeneratorBehaviour : MonoBehaviour
{
    // Common properties: they are the same for all virtual maps and generation algorithms
    // Sizes
	public static int MAX_SIZE = 99;
	public int MapWidth = 2;
	public int MapHeight = 2;

    // Algorithm
    public int seed = 0;
    public bool useSeed = false;

    // Multiple storeys properties
    public MapDimensionsType mapDimensionsType;
    public int numberOfStoreys = 1;
    public bool multiStorey = false;

    // Internal variables
	[SerializeField]
	GameObject rootMapGo = null;

	[SerializeField]
	public PhysicalMap physicalMap;
	
	[SerializeField]
	public PhysicalMapBehaviour physicalMapBehaviour;
	
	[SerializeField]
	public MapInterpreterBehaviour interpreterBehaviour;

    [SerializeField]
    public VirtualMapGeneratorBehaviour virtualMapGeneratorBehaviour;

	[SerializeField]
	public bool isCurrentlyGenerating = false;

    [SerializeField]
    private VirtualMap[] virtualMaps;	// One for each storey

    [SerializeField]
    private MapInterpreter mapInterpreter;


    // Debug & Testing
    DateTime preDate;
    DateTime postDate;
    public bool printTimings = false;
    public uint lastUsedSeed;
	

	public void Generate ()
	{
		isCurrentlyGenerating = true;

        // Make sure we have a VirtualMapGeneratorBheaviour component
        virtualMapGeneratorBehaviour = gameObject.GetComponent<VirtualMapGeneratorBehaviour>();
        if (virtualMapGeneratorBehaviour == null)
        {
            Debug.LogError("You need to attach a VirtualMapGeneratorBehaviour to this gameObject to enable generation!", this);
            return;
        }

		// Make sure we have a PhysicalMapBehaviour component
		physicalMapBehaviour = gameObject.GetComponent<PhysicalMapBehaviour>();
		if (physicalMapBehaviour == null) {
			Debug.LogError("You need to attach a PhysicalMapBehaviour to this gameObject to enable generation!",this);
			return;
		}
		
		// Make sure we have a MapInterpreterBehaviour component
		interpreterBehaviour = gameObject.GetComponent<MapInterpreterBehaviour>();
		if (interpreterBehaviour == null) {
			Debug.LogError("You need to attach a MapInterpreterBehaviour to this gameObject to enable generation!",this);
			return;
		}
		
		// Remove the existing map 
		if (rootMapGo != null){
			if (physicalMap != null) {
				physicalMap.CleanUp();
				DestroyImmediate (physicalMap);
			}

			if (Application.isPlaying) Destroy(rootMapGo);
			else DestroyImmediate(rootMapGo);	
			virtualMaps = null;
			physicalMap = null;
		}

		// Remove any other existing children as well
		foreach(Transform childTr in this.transform){
			DestroyImmediate (childTr.gameObject);
		}

		
		if (printTimings) preDate = System.DateTime.Now;
		
		if (!ForceCommonSenseOptions()) return;

		physicalMapBehaviour.MeasureSizes();
		SetGeneratorValues();

        virtualMapGeneratorBehaviour.Initialise();
        lastUsedSeed = virtualMapGeneratorBehaviour.InitialiseSeed(useSeed, seed);
        virtualMaps = virtualMapGeneratorBehaviour.GenerateAllMaps(MapWidth, MapHeight, numberOfStoreys);

        mapInterpreter = interpreterBehaviour.Generate();

        physicalMap = physicalMapBehaviour.Generate(virtualMaps, this, mapInterpreter);

		this.rootMapGo = physicalMap.rootMapGo;
			
		if (printTimings){
			postDate = System.DateTime.Now;
			TimeSpan timeDifference = postDate.Subtract (preDate);
			Debug.Log ("Generated in " + timeDifference.TotalMilliseconds.ToString () + " ms");
		}

		BroadcastMessage("DungeonGenerated",SendMessageOptions.DontRequireReceiver);
		isCurrentlyGenerating = false;

	}


    private bool ForceCommonSenseOptions()
    {
        this.virtualMapGeneratorBehaviour.ForceCommonSenseOptions();
		return this.physicalMapBehaviour.CheckDefaults();
	}

    private void SetGeneratorValues()
    {
        GeneratorValues.seed = seed;
        GeneratorValues.multiStorey = multiStorey;
        GeneratorValues.numberOfStoreys = numberOfStoreys;
    }
	
	public PhysicalMap GetPhysicalMap(){
		return this.physicalMap;	
	}
	public VirtualMap[] GetVirtualMaps(){
		return this.virtualMaps;	
	}


    public bool HasGeneratedDungeon()
    {
        return this.rootMapGo != null && this.mapInterpreter != null;
    }

}
	
