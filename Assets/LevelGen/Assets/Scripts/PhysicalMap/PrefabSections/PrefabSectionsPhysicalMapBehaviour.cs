using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabSectionsPhysicalMapBehaviour : PhysicalMapBehaviour<GameObject, GameObjectChoice>
{

	public bool manualTileSizes;
    public float tileSize;
    public float wallHeight;
    public float storeySeparationHeight = 1;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
 #endif
    public GameObjectChoice[] sectionUVariations;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionIVariations;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionLVariations;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionTVariations;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionXVariations;

    /* NO! LADDERS DO NOT WORK CORRECTLY WITH SECTIONS!
#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionLadderVariations;
    */

    // Internal options
    override public bool SupportsThreeDeeMap { get { return false; } }  // No 3D, does not make sense with limited tiles.


	override public void MeasureSizes(){
		if(manualTileSizes) return;
		this.tileSize = MeasureTileSize();
        this.wallHeight = MeasureWallHeight();
	}
	
	//override public bool ForcedOrientation{get{return true;}}

	public float MeasureTileSize(){	
        GameObject prefab = GetPrefab(VirtualCell.CellType.CorridorFloorU); // Get the first one (U)

        MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
        if (meshFilter != null) return meshFilter.sharedMesh.bounds.size.z;

        DaedalusDebugUtils.Assert(false, "Cannot Measure Tile Size For Prefab " + prefab.name);
        return 0;
    }

    public float MeasureWallHeight()
    {
        Bounds bounds;
        bounds = GetPrefab(VirtualCell.CellType.CorridorFloorU).GetComponent<MeshFilter>().sharedMesh.bounds;
        return bounds.size.y;
    }


    override protected GameObjectChoice[] GetVariations(VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation = VirtualMap.DirectionType.None)
    {
    
        // Treats floors as prefab sections
        GameObjectChoice[] type_choices = null;
        switch (cell_type)
        {
            case VirtualCell.CellType.CorridorFloorU: type_choices = sectionUVariations; break;
            case VirtualCell.CellType.CorridorFloorI: type_choices = sectionIVariations; break;
            case VirtualCell.CellType.CorridorFloorL: type_choices = sectionLVariations; break;
            case VirtualCell.CellType.CorridorFloorT: type_choices = sectionTVariations; break;
            case VirtualCell.CellType.CorridorFloorX: type_choices = sectionXVariations; break;
            //case VirtualCell.CellType.Ladder:         type_choices = sectionLadderVariations; break;
            default: type_choices = base.GetVariations(cell_type, orientation); break;
        }
        return type_choices;
    }


    override public bool CheckDefaults()
    {
        rockVariations = CheckDefault("Daedalus_Resources/3D/Sections/DefaultSectionRock", rockVariations);

        sectionUVariations = CheckDefault("Daedalus_Resources/3D/Sections/DefaultSectionU", sectionUVariations);
        sectionIVariations = CheckDefault("Daedalus_Resources/3D/Sections/DefaultSectionI", sectionIVariations);
        sectionLVariations = CheckDefault("Daedalus_Resources/3D/Sections/DefaultSectionL", sectionLVariations);
        sectionTVariations = CheckDefault("Daedalus_Resources/3D/Sections/DefaultSectionT", sectionTVariations);
        sectionXVariations = CheckDefault("Daedalus_Resources/3D/Sections/DefaultSectionX", sectionXVariations);

		if (!entrancePrefab) entrancePrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultEntrance",typeof(GameObject)) as GameObject;
		if (!exitPrefab) exitPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultExit",typeof(GameObject)) as GameObject;

		//mapPlaneOrientation = MapPlaneOrientation.XZ;
		return true;
	}

	override public PhysicalMap Generate(VirtualMap[] maps, GeneratorBehaviour generator, MapInterpreter interpreter){
        PrefabSectionsPhysicalMap physMap = ScriptableObject.CreateInstance<PrefabSectionsPhysicalMap>();
		physMap.Initialise(maps,generator,interpreter);	
		physMap.behaviour = this;
		physMap.Generate();
		return physMap;	
	}
}