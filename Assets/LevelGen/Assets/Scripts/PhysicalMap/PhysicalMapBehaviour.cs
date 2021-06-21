using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

// From Unity 4.3 release notes:
// Editor: PropertyDrawer attributes on members that are arrays are now applied to each element in the array rather than the array as a whole.
// This was always the intention since there is no other way to apply attributes to array elements,
// but it didn't work correctly before. Apologies for the inconvenience to anyone who relied on the unintended behavior for custom drawing of arrays.


public abstract class PhysicalMapBehaviour<T,S> : PhysicalMapBehaviour where S : GenericChoice<T>, new() where T: class {

	// Variations

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif
	public S[] corridorWallVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorFloorVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorColumnVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomWallVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomFloorVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomColumnVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] insideRoomColumnVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] doorVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomDoorVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] passageColumnVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] rockVariations;

	/*
	 * Advanced floors
	 */
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorFloorUVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorFloorIVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorFloorLVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorFloorTVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorFloorXVariations;


	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomFloorInsideVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomFloorBorderVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomFloorCornerVariations;

	/*
	 * Advanced walls
	 */
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorWallOVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorWallUVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorWallIVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorWallLVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorWallTVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] corridorWallXVariations;


	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomWallOVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomWallUVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomWallIVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomWallLVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomWallTVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] roomWallXVariations;



	// Perimeter
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] perimeterWallVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public S[] perimeterColumnVariations;

		

    virtual protected S[] GetVariations(VirtualCell.CellType cell_type)
    {
        return GetVariations(cell_type, VirtualMap.DirectionType.None);
    }

    virtual protected S[] GetVariations(VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation)
    {
		S[] type_choices = null;
		switch(cell_type){
			case VirtualCell.CellType.CorridorWall: 	type_choices = corridorWallVariations;		break;
			case VirtualCell.CellType.CorridorFloor: 	type_choices = corridorFloorVariations;		break;
			case VirtualCell.CellType.CorridorColumn:	type_choices = corridorColumnVariations;	break;
			case VirtualCell.CellType.RoomWall: 		type_choices = roomWallVariations;			break;
			case VirtualCell.CellType.RoomFloor: 		type_choices = roomFloorVariations;			break;
			case VirtualCell.CellType.RoomColumn:		type_choices = roomColumnVariations;		break;
			case VirtualCell.CellType.InsideRoomColumn:	type_choices = insideRoomColumnVariations;	break;
			case VirtualCell.CellType.Door:				type_choices = doorVariations;				break;
			case VirtualCell.CellType.RoomDoor:			type_choices = roomDoorVariations;			break;
			case VirtualCell.CellType.PassageColumn:	type_choices = passageColumnVariations;		break;
			case VirtualCell.CellType.Rock:				type_choices = rockVariations;				break;
			case VirtualCell.CellType.CorridorFloorU:	type_choices = corridorFloorUVariations;	break;
			case VirtualCell.CellType.CorridorFloorI:	type_choices = corridorFloorIVariations;	break;
			case VirtualCell.CellType.CorridorFloorL:	type_choices = corridorFloorLVariations;	break;
			case VirtualCell.CellType.CorridorFloorT:	type_choices = corridorFloorTVariations;	break;
			case VirtualCell.CellType.CorridorFloorX:	type_choices = corridorFloorXVariations;	break;
			case VirtualCell.CellType.RoomFloorInside:	type_choices = roomFloorInsideVariations;	break;
			case VirtualCell.CellType.RoomFloorBorder:	type_choices = roomFloorBorderVariations;	break;
			case VirtualCell.CellType.RoomFloorCorner:	type_choices = roomFloorCornerVariations;	break;
			case VirtualCell.CellType.PerimeterWall:	type_choices = perimeterWallVariations;		break;
			case VirtualCell.CellType.PerimeterColumn:	type_choices = perimeterColumnVariations;	break;
			case VirtualCell.CellType.CorridorWallO:	type_choices = corridorWallOVariations;		break;
			case VirtualCell.CellType.CorridorWallU:	type_choices = corridorWallUVariations;		break;
			case VirtualCell.CellType.CorridorWallI:	type_choices = corridorWallIVariations;		break; 
			case VirtualCell.CellType.CorridorWallL:	type_choices = corridorWallLVariations;		break;
			case VirtualCell.CellType.CorridorWallT:	type_choices = corridorWallTVariations;		break;
			case VirtualCell.CellType.CorridorWallX:	type_choices = corridorWallXVariations;		break;
			case VirtualCell.CellType.RoomWallO:		type_choices = roomWallOVariations;			break;
			case VirtualCell.CellType.RoomWallU:		type_choices = roomWallUVariations;			break;
			case VirtualCell.CellType.RoomWallI:		type_choices = roomWallIVariations;			break;
			case VirtualCell.CellType.RoomWallL:		type_choices = roomWallLVariations;			break;
			case VirtualCell.CellType.RoomWallT:		type_choices = roomWallTVariations;			break;
			case VirtualCell.CellType.RoomWallX:		type_choices = roomWallXVariations;			break;
			default: Debug.LogError("No prefab for cell type " + cell_type); break;
		}	
		return type_choices;
	}
	
		
	protected S[] CheckDefault(string defaultName, S[] variations){
		if (variations == null || variations.Length == 0) {
			variations = new S[1];
			variations[0] = new S();
			variations[0].choice = GetDefault(defaultName);
			variations[0].weight = 1;
		} else {
			foreach(S v in variations) {
				if (v.choice == null) v.choice =  GetDefault(defaultName);
				if (v.weight < 1) v.weight = 1;
			}
		}
//		Debug.Log(variations);
		return variations;
	}

	virtual protected T GetDefault(string defaultName, VirtualMap.DirectionType orientation){
        T default_object = Resources.Load(defaultName, typeof(T)) as T;
        //if (default_object == null)	Debug.LogWarning ("No default for type " + defaultName + " could be found! Make sure to add a variation for it!");
        DaedalusDebugUtils.Assert(default_object != null, "No default of name " + defaultName + " could be found!", this);
        return default_object;
    }

    virtual protected T GetDefault(string defaultName){
        return GetDefault(defaultName, VirtualMap.DirectionType.None);
	}


    public T GetPrefab(VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation = VirtualMap.DirectionType.None)
    {
        S[] type_choices = null;
        type_choices = GetVariations(cell_type, orientation);
        if (type_choices == null) return default(T);

        // Get one with a weighted random
        int tot = 0;
        foreach (S choice in type_choices)
        {
            tot += choice.weight;
        }
        int rnd = DungeonGenerator.Random.Instance.Next(1, tot);
        //		Debug.Log(cell_type + " " + tot + " " + rnd);

        int current = 0;
        foreach (S choice in type_choices)
        {
            current += choice.weight;
            if (rnd <= current)
            {
                //				Debug.Log("YTYPE " + cell_type + " CHOICE: " + choice.choice);
                return choice.choice;
            }
        }
        return default(T);
    }

}
	
[RequireComponent (typeof (GeneratorBehaviour))]
public abstract class PhysicalMapBehaviour : MonoBehaviour {
	
	// Common parameters
	public bool enabledBatching = false;
    public bool createPlayer = false;
	public GameObject playerPrefab;
	public MapPlaneOrientation mapPlaneOrientation;

    // Entrance and exit prefab, placed on the starting and ending floor tiles
    public bool createEntranceAndExit = false;
	public GameObject entrancePrefab;
	public GameObject exitPrefab;
	
	
	
	// These should be set by derived classes
	virtual public bool AutomaticBatching{get{ return false;}}			// If true, this physical map performs batching already and not further batching is required
	virtual public bool AutomaticOrientation{get{ return false;}}		// If true, this physical map cannot be orientated
	virtual public bool ForcedOrientation{get{ return false;}}			// If true, this physical map has its orientation forced
	virtual public bool SupportsThreeDeeMap{get {return false;}}
	virtual public void MeasureSizes(){}
	abstract public bool CheckDefaults();	// Returns false if something is amiss, so that generation does not continue!
	abstract public PhysicalMap Generate(VirtualMap[] maps, GeneratorBehaviour generator, MapInterpreter interpreter);

}
