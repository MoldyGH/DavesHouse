using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabOrientedSectionsPhysicalMapBehaviour : PhysicalMapBehaviour<GameObject, GameObjectChoice>
{

	public bool manualTileSizes;
    public float tileSize;
    public float wallHeight;
    public float storeySeparationHeight = 1;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
 #endif
    public GameObjectChoice[] sectionUVariations_N;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionUVariations_E;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionUVariations_W;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionUVariations_S;


#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionIVariations_NS;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionIVariations_WE;



#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionLVariations_NE;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionLVariations_NW;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionLVariations_SE;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionLVariations_SW;



#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionTVariations_XS;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionTVariations_XW;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionTVariations_XN;

#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionTVariations_XE;



#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
#endif
    public GameObjectChoice[] sectionXVariations;


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
        if (orientation == VirtualMap.DirectionType.None) orientation = VirtualMap.DirectionType.North;
        // Treats floors as prefab sections
        GameObjectChoice[] type_choices = null;
        switch (cell_type)
        {
            case VirtualCell.CellType.CorridorFloorU:
                switch (orientation)
                {
                    case VirtualMap.DirectionType.East: type_choices = sectionUVariations_E; break;
                    case VirtualMap.DirectionType.North: type_choices = sectionUVariations_N; break;
                    case VirtualMap.DirectionType.West: type_choices = sectionUVariations_W; break;
                    case VirtualMap.DirectionType.South: type_choices = sectionUVariations_S; break;
                }
                break;

            case VirtualCell.CellType.CorridorFloorI: 
                switch (orientation)
                {
                    case VirtualMap.DirectionType.East: type_choices = sectionIVariations_WE; break;
                    case VirtualMap.DirectionType.North: type_choices = sectionIVariations_NS; break;
                    case VirtualMap.DirectionType.West: type_choices = sectionIVariations_WE; break;
                    case VirtualMap.DirectionType.South: type_choices = sectionIVariations_NS; break;
                }
                break;

            case VirtualCell.CellType.CorridorFloorL: 
                switch (orientation)
                {
                    case VirtualMap.DirectionType.East: type_choices = sectionLVariations_SE; break;
                    case VirtualMap.DirectionType.North: type_choices = sectionLVariations_NE; break;
                    case VirtualMap.DirectionType.West:  type_choices = sectionLVariations_NW; break;
                    case VirtualMap.DirectionType.South: type_choices = sectionLVariations_SW; break;
                }
                break;
                
            case VirtualCell.CellType.CorridorFloorT:
                switch (orientation)
                {
                    case VirtualMap.DirectionType.East: type_choices = sectionTVariations_XE; break;
                    case VirtualMap.DirectionType.North: type_choices = sectionTVariations_XN; break;
                    case VirtualMap.DirectionType.West:  type_choices = sectionTVariations_XW; break;
                    case VirtualMap.DirectionType.South: type_choices = sectionTVariations_XS; break;
                }
                break;
            case VirtualCell.CellType.CorridorFloorX: type_choices = sectionXVariations; break;
            default: type_choices = base.GetVariations(cell_type, orientation); break;
        }
        return type_choices;
    }


    override public bool CheckDefaults()
    {
        rockVariations = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_Rock", rockVariations);

        sectionUVariations_N = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_U_N", sectionUVariations_N);
        sectionUVariations_E = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_U_E", sectionUVariations_E);
        sectionUVariations_W = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_U_W", sectionUVariations_W);
        sectionUVariations_S = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_U_S", sectionUVariations_S);

        sectionIVariations_NS = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_I_NS", sectionIVariations_NS);
        sectionIVariations_WE = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_I_WE", sectionIVariations_WE);

        sectionLVariations_NE = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_L_NE", sectionLVariations_NE);
        sectionLVariations_NW = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_L_NW", sectionLVariations_NW);
        sectionLVariations_SE = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_L_SE", sectionLVariations_SE);
        sectionLVariations_SW = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_L_SW", sectionLVariations_SW);

        sectionTVariations_XE = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_T_E", sectionTVariations_XE);
        sectionTVariations_XN = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_T_N", sectionTVariations_XN);
        sectionTVariations_XW = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_T_W", sectionTVariations_XW);
        sectionTVariations_XS = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_T_S", sectionTVariations_XS);

        sectionXVariations = CheckDefault("Daedalus_Resources/3D/OrientedSections/DefaultOrientedSection_X", sectionXVariations);

		if (!entrancePrefab) entrancePrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultEntrance",typeof(GameObject)) as GameObject;
		if (!exitPrefab) exitPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultExit",typeof(GameObject)) as GameObject;

		//mapPlaneOrientation = MapPlaneOrientation.XZ;
		return true;
	}

	override public PhysicalMap Generate(VirtualMap[] maps, GeneratorBehaviour generator, MapInterpreter interpreter){
        PrefabOrientedSectionsPhysicalMap physMap = ScriptableObject.CreateInstance<PrefabOrientedSectionsPhysicalMap>();
		physMap.Initialise(maps,generator,interpreter);	
		physMap.behaviour = this;
		physMap.Generate();
		return physMap;	
	}
}