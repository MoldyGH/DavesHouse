using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Note: tileSize must be equal to wallSize+columnSize
public class Meshes3DPhysicalMapBehaviour : PhysicalMapBehaviour<GameObject,GameObjectChoice> {

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public GameObjectChoice[] corridorCeilingVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public GameObjectChoice[] roomCeilingVariations;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public GameObjectChoice[] ladderVariations;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif	
	public GameObjectChoice[] ladder2Variations;
	
	public bool manualTileSizes; 

	public float tileSize;
	public float columnSize;
	public float wallSize;
	public float wallHeight;
	public float storeySeparationHeight = 1;

	// Internal options
	override public bool SupportsThreeDeeMap{get{return true;}}

	
	override public void MeasureSizes(){
		this.tileSize = MeasureTileSize();
		this.columnSize = MeasureColumnSize();
		this.wallSize = MeasureWallSize();	
		this.wallHeight = MeasureWallHeight();	
	}
	
	public float MeasureTileSize(){	
		GameObject prefab = GetPrefab(VirtualCell.CellType.CorridorFloor);
        if (prefab == null) prefab = GetPrefab(VirtualCell.CellType.CorridorFloorI); // We use the I one if we cannot fetch the floor!

        DaedalusDebugUtils.Assert(prefab != null, "No prefab was chosen for CorridorFloor or CorridorFloorI! We need one of those to measure tile size!");

		MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
		if (meshFilter != null) return meshFilter.sharedMesh.bounds.size.z;

		DaedalusDebugUtils.Assert(false,"Cannot Measure Tile Size For Prefab " + prefab.name);
		return 0;
	}
	
	public float MeasureColumnSize(){
		Bounds bounds;
		bounds = GetPrefab(VirtualCell.CellType.CorridorColumn).GetComponent<MeshFilter>().sharedMesh.bounds;
		return bounds.size.z;
	}

	public float MeasureWallSize(){
		Bounds bounds;
		bounds = GetPrefab(VirtualCell.CellType.CorridorWall).GetComponent<MeshFilter>().sharedMesh.bounds;
		return bounds.size.z;
	}
	
	public float MeasureWallHeight(){
		Bounds bounds;
		bounds = GetPrefab(VirtualCell.CellType.CorridorWall).GetComponent<MeshFilter>().sharedMesh.bounds;
		return bounds.size.y;
	}
	
	override public bool CheckDefaults(){		
		
		// TODO: Instead of checking the type, we should use inheritance: there should be a PhysicalMap-MapInterpreter pair class that defines the default
		MapInterpreterBehaviour interpreterBehaviour = GetComponent<MapInterpreterBehaviour>();
		
		if (interpreterBehaviour is StandardMapInterpreterBehaviour){
			
			corridorWallVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorWall",corridorWallVariations);
			corridorFloorVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorFloor",corridorFloorVariations);
			corridorColumnVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorColumn",corridorColumnVariations);
			corridorCeilingVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorCeiling",corridorCeilingVariations);
			
			roomWallVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomWall",roomWallVariations);
			roomFloorVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomFloor",roomFloorVariations);
			roomColumnVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumn",roomColumnVariations);
			insideRoomColumnVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumn",insideRoomColumnVariations);
			roomCeilingVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomCeiling",roomCeilingVariations);
			
			corridorFloorUVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorU",corridorFloorUVariations);
			corridorFloorIVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorI",corridorFloorIVariations);
			corridorFloorLVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorL",corridorFloorLVariations);
			corridorFloorTVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorT",corridorFloorTVariations);
			corridorFloorXVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorX",corridorFloorXVariations);
			
			roomFloorInsideVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomFloor",roomFloorInsideVariations);
			roomFloorCornerVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomFloorCorner",roomFloorCornerVariations);
			roomFloorBorderVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomFloorBorder",roomFloorBorderVariations);

			corridorWallOVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorColumnO",corridorWallOVariations);
			corridorWallUVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorColumnU",corridorWallUVariations);
			corridorWallIVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorColumnI",corridorWallIVariations);
			corridorWallLVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorColumnL",corridorWallLVariations);
			corridorWallTVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorColumnT",corridorWallTVariations);
			corridorWallXVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultCorridorColumnX",corridorWallXVariations);
			
			roomWallOVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumnO",roomWallOVariations);
			roomWallUVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumnU",roomWallUVariations);
			roomWallIVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumnI",roomWallIVariations);
			roomWallLVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumnL",roomWallLVariations);
			roomWallTVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumnT",roomWallTVariations);
			roomWallXVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomColumnX",roomWallXVariations);

			doorVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultDoor",doorVariations);
			roomDoorVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRoomDoor",roomDoorVariations);
			passageColumnVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultDoorColumn",passageColumnVariations);
			rockVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultRock",rockVariations);

			perimeterWallVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultPerimeter",perimeterWallVariations);
			perimeterColumnVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultPerimeterColumn",perimeterColumnVariations);

			ladderVariations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultLadder1",ladderVariations);
			ladder2Variations = CheckDefault("Daedalus_Resources/3D/StandardMap/DefaultLadder2",ladder2Variations);

		} else if (interpreterBehaviour is TileMapInterpreterBehaviour){
			corridorWallVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorWall",corridorWallVariations);
			corridorFloorVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorFloor",corridorFloorVariations);
			corridorColumnVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorColumn",corridorColumnVariations);
			corridorCeilingVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorCeiling",corridorCeilingVariations);
			
			roomWallVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomWall",roomWallVariations);
			roomFloorVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomFloor",roomFloorVariations);
			roomColumnVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomColumn",roomColumnVariations);
			insideRoomColumnVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomInsideColumn",insideRoomColumnVariations);
			roomCeilingVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomCeiling",roomCeilingVariations);
			
			corridorFloorUVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorU",corridorFloorUVariations);
			corridorFloorIVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorI",corridorFloorIVariations);
			corridorFloorLVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorL",corridorFloorLVariations);
			corridorFloorTVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorT",corridorFloorTVariations);
			corridorFloorXVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorX",corridorFloorXVariations);
			
			roomFloorInsideVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomFloor",roomFloorInsideVariations);
			roomFloorCornerVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomFloorCorner",roomFloorCornerVariations);
			roomFloorBorderVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomFloorBorder",roomFloorBorderVariations);

			corridorWallOVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorWallO",corridorWallOVariations);
			corridorWallUVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorWallU",corridorWallUVariations);
			corridorWallIVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorWallI",corridorWallIVariations);
			corridorWallLVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorWallL",corridorWallLVariations);
			corridorWallTVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorWallT",corridorWallTVariations);
			corridorWallXVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockCorridorWallX",corridorWallXVariations);
			
			roomWallOVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomWallO",roomWallOVariations);
			roomWallUVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomWallU",roomWallUVariations);
			roomWallIVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomWallI",roomWallIVariations);
			roomWallLVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomWallL",roomWallLVariations);
			roomWallTVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomWallT",roomWallTVariations);
			roomWallXVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomWallX",roomWallXVariations);
			
			perimeterWallVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockPerimeterWall",perimeterWallVariations);
			perimeterColumnVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockPerimeterColumn",perimeterColumnVariations);

			doorVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockDoor",doorVariations);
			roomDoorVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRoomDoor",roomDoorVariations);
			passageColumnVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockDoorColumn",passageColumnVariations);
			rockVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockRock",rockVariations);	

			ladderVariations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockLadder1",ladderVariations);
			ladder2Variations = CheckDefault("Daedalus_Resources/3D/Tiles3D/DefaultBlockLadder2",ladder2Variations);
		}
			
		if (!entrancePrefab) entrancePrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultEntrance",typeof(GameObject)) as GameObject;
		if (!exitPrefab) exitPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultExit",typeof(GameObject)) as GameObject;
			
		return true;
	}

    override protected GameObjectChoice[] GetVariations(VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation = VirtualMap.DirectionType.None)
    {
		GameObjectChoice[] type_choices = null;
        //Debug.Log(cell_type);
		switch(cell_type){
			case VirtualCell.CellType.CorridorCeiling: 	type_choices = corridorCeilingVariations;		break;
			case VirtualCell.CellType.RoomCeiling: 		type_choices = roomCeilingVariations;			break;
			case VirtualCell.CellType.Ladder:			type_choices = ladderVariations;				break;
			case VirtualCell.CellType.Ladder2:			type_choices = ladder2Variations;				break;
            default: type_choices = base.GetVariations(cell_type, orientation); break;
		}	
		return type_choices;
	}

	override public PhysicalMap Generate(VirtualMap[] maps, GeneratorBehaviour generator, MapInterpreter interpreter){
		Meshes3DPhysicalMap physMap = ScriptableObject.CreateInstance<Meshes3DPhysicalMap>();
		physMap.Initialise(maps,generator,interpreter);	
		physMap.behaviour = this;
		physMap.Generate();
		return physMap;	
	}
}