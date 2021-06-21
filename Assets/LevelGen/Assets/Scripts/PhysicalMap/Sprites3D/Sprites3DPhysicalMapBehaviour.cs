using UnityEngine;
using System.Collections;

public class Sprites3DPhysicalMapBehaviour : PhysicalMapBehaviour<Texture2D,Texture2DChoice> {
	
	public int tileSize;
	public GameObject tilePrefab;
	
	override public PhysicalMap Generate(VirtualMap[] maps, GeneratorBehaviour generator, MapInterpreter interpreter){
		Sprites3DPhysicalMap physMap = ScriptableObject.CreateInstance<Sprites3DPhysicalMap>();
		physMap.Initialise(maps,generator,interpreter);
		physMap.behaviour = this;
		physMap.Generate();
		return physMap;
	}

	
	override public void MeasureSizes(){
		this.tileSize = MeasureTileSize();
	}
	
	public int MeasureTileSize(){	
		Bounds bounds;
		bounds = tilePrefab.GetComponent<MeshFilter>().sharedMesh.bounds;
		return (int)bounds.size.z;
	}
	
	override public bool CheckDefaults(){
		
		if (tilePrefab == null) {
			tilePrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultTile",typeof(GameObject)) as GameObject;
		}

		// Default assets if not specified		
		corridorWallVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorWallTexture",corridorWallVariations);
		corridorFloorVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorFloorTexture",corridorFloorVariations);
		corridorColumnVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorColumnTexture",corridorColumnVariations);
		
		corridorFloorUVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorFloorUTexture",corridorFloorUVariations);
		corridorFloorIVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorFloorITexture",corridorFloorIVariations);
		corridorFloorLVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorFloorLTexture",corridorFloorLVariations);
		corridorFloorTVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorFloorTTexture",corridorFloorTVariations);
		corridorFloorXVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorFloorXTexture",corridorFloorXVariations);


		roomWallVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomWallTexture",roomWallVariations);
		roomFloorVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomFloorTexture",roomFloorVariations);
		roomColumnVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomColumnTexture",roomColumnVariations);
		insideRoomColumnVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomColumnInsideTexture",insideRoomColumnVariations);
		
		roomFloorInsideVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomFloorTexture",roomFloorInsideVariations);
		roomFloorCornerVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomFloorCornerTexture",roomFloorCornerVariations);
		roomFloorBorderVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomFloorBorderTexture",roomFloorBorderVariations);

		corridorWallOVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorWallOTexture",corridorWallOVariations);
		corridorWallUVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorWallUTexture",corridorWallUVariations);
		corridorWallIVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorWallITexture",corridorWallIVariations);
		corridorWallLVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorWallLTexture",corridorWallLVariations);
		corridorWallTVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorWallTTexture",corridorWallTVariations);
		corridorWallXVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultCorridorWallXTexture",corridorWallXVariations);
		
		roomWallOVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomWallOTexture",roomWallOVariations);
		roomWallUVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomWallUTexture",roomWallUVariations);
		roomWallIVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomWallITexture",roomWallIVariations);
		roomWallLVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomWallLTexture",roomWallLVariations);
		roomWallTVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomWallTTexture",roomWallTVariations);
		roomWallXVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomWallXTexture",roomWallXVariations);


		perimeterWallVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultPerimeterWallTexture",perimeterWallVariations);
		perimeterColumnVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultPerimeterColumnTexture",perimeterColumnVariations);

		doorVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultDoorTexture",doorVariations);
		roomDoorVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRoomDoorTexture",roomDoorVariations);

		passageColumnVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultDoorColumnTexture",passageColumnVariations);
		
		rockVariations = CheckDefault("Daedalus_Resources/Shared/Textures/DefaultRockTexture",rockVariations);
		
		if (!entrancePrefab) entrancePrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultEntrance",typeof(GameObject)) as GameObject;
		if (!exitPrefab) exitPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultExit",typeof(GameObject)) as GameObject;
		
		
		if (!tilePrefab) {
			Debug.Log ("You must set a tilePrefab in the PhysicalMap!");
			return false;
		}
		return true;
	}

}