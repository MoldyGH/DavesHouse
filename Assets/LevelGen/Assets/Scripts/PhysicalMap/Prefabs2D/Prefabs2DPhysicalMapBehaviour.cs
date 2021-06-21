using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Note: tileSize must be equal to wallSize+columnSize
public class Prefabs2DPhysicalMapBehaviour : PhysicalMapBehaviour<GameObject,GameObjectChoice> {

	public bool manualTileSizes;

	public float tileSize;
	public float columnSize;
	public float wallSize;

	override public void MeasureSizes(){
		if(manualTileSizes) return;
		this.tileSize = MeasureTileSize();
		this.columnSize = MeasureColumnSize();
		this.wallSize = MeasureWallSize();	
	}
	
	override public bool ForcedOrientation{get{return true;}}

	public float MeasureTileSize(){	
		return MeasureSize(VirtualCell.CellType.CorridorFloor);
	}
	public float MeasureColumnSize(){
		return MeasureSize(VirtualCell.CellType.CorridorColumn);
	}
	public float MeasureWallSize(){
		return MeasureSize(VirtualCell.CellType.CorridorWall);
	}

	public float MeasureSize(VirtualCell.CellType type){
		GameObject prefab = GetPrefab(type);
		
		BoxCollider2D collider = prefab.GetComponent<BoxCollider2D>();
		if (collider != null) return collider.size.x;
		
		DaedalusDebugUtils.Assert(false,"Cannot Measure Tile Size For Prefab " + prefab.name);
		return 0;
	}


	override public bool CheckDefaults(){		
		MapInterpreterBehaviour interpreterBehaviour = GetComponent<MapInterpreterBehaviour>();
		if (interpreterBehaviour is TileMapInterpreterBehaviour){
			corridorWallVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorWall",corridorWallVariations);
			corridorFloorVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorFloor",corridorFloorVariations);
			corridorColumnVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorColumn",corridorColumnVariations);

			roomWallVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomWall",roomWallVariations);
			roomFloorVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomFloor",roomFloorVariations);
			roomColumnVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomColumn",roomColumnVariations);
			insideRoomColumnVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomInsideColumn",insideRoomColumnVariations);

			corridorFloorUVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorFloorU",corridorFloorUVariations);
			corridorFloorIVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorFloorI",corridorFloorIVariations);
			corridorFloorLVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorFloorL",corridorFloorLVariations);
			corridorFloorTVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorFloorT",corridorFloorTVariations);
			corridorFloorXVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorFloorX",corridorFloorXVariations);
			
			roomFloorInsideVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomFloor",roomFloorInsideVariations);
			roomFloorCornerVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomFloorCorner",roomFloorCornerVariations);
			roomFloorBorderVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomFloorBorder",roomFloorBorderVariations);
			
			perimeterWallVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/PerimeterWall",perimeterWallVariations);
			perimeterColumnVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/PerimeterColumn",perimeterColumnVariations);

			corridorWallOVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorWallO",corridorWallOVariations);
			corridorWallUVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorWallU",corridorWallUVariations);
			corridorWallIVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorWallI",corridorWallIVariations);
			corridorWallLVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorWallL",corridorWallLVariations);
			corridorWallTVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorWallT",corridorWallTVariations);
			corridorWallXVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/CorridorWallX",corridorWallXVariations);
			
			roomWallOVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomWallO",roomWallOVariations);
			roomWallUVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomWallU",roomWallUVariations);
			roomWallIVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomWallI",roomWallIVariations);
			roomWallLVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomWallL",roomWallLVariations);
			roomWallTVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomWallT",roomWallTVariations);
			roomWallXVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomWallX",roomWallXVariations);

			doorVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/Door",doorVariations);
			roomDoorVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/RoomDoor",roomDoorVariations);
			passageColumnVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/DoorColumn",passageColumnVariations);
			rockVariations = CheckDefault("Daedalus_Resources/2D/Prefabs2D/Rock",rockVariations);	
		}
			
		if (!entrancePrefab) entrancePrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultEntrance",typeof(GameObject)) as GameObject;
		if (!exitPrefab) exitPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultExit",typeof(GameObject)) as GameObject;
        if (!playerPrefab) playerPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/Player2D", typeof(GameObject)) as GameObject;

		mapPlaneOrientation = MapPlaneOrientation.XY;

		return true;
	}

	override public PhysicalMap Generate(VirtualMap[] maps, GeneratorBehaviour generator, MapInterpreter interpreter){
		Prefabs2DPhysicalMap physMap = ScriptableObject.CreateInstance<Prefabs2DPhysicalMap>();
		physMap.Initialise(maps,generator,interpreter);	
		physMap.behaviour = this;
		physMap.Generate();
		return physMap;	
	}
}