using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
	
public class Sprites2DPhysicalMapBehaviour : PhysicalMapBehaviour<Sprite,SpriteChoice> {

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif
	public SpriteChoice[] fake3DCorridorWallAbove;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif
	public SpriteChoice[] fake3DCorridorWallFront;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif
	public SpriteChoice[] fake3DRoomWallAbove;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif
	public SpriteChoice[] fake3DRoomWallFront;
	
//	#if UNITY_4_2
//	[List(showSize = false, showListLabel = true, showElementLabels = false)]
//	#endif
//	public SpriteChoice[] doorsHorizontalTop;
	
	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif
	public SpriteChoice[] doorsHorizontalBottom;

	#if UNITY_4_2
	[List(showSize = false, showListLabel = true, showElementLabels = false)]
	#endif
	public SpriteChoice[] doorsVertical;


	public float tileSize = 4;
	public GameObject spritePrefab;
	
	override public bool ForcedOrientation{get{return true;}}

	
	override public PhysicalMap Generate(VirtualMap[] maps, GeneratorBehaviour generator, MapInterpreter interpreter){
		Sprites2DPhysicalMap physMap = ScriptableObject.CreateInstance<Sprites2DPhysicalMap>();
		physMap.Initialise(maps,generator,interpreter);
		physMap.behaviour = this;
		physMap.Generate();
		return physMap;
	} 
	
	
	Dictionary<string,Sprite> loadedSpritesDict;
	override public bool CheckDefaults(){

		if (spritePrefab == null) spritePrefab = Resources.Load ("Daedalus_Resources/2D/2DSpritePrefab") as GameObject;

		// We need to load the sprites in a special way, since we need to get also the sprite sheets
		loadedSpritesDict =  new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Daedalus_Resources/2D");
		for(int ii=0; ii< sprites.Length; ii++) loadedSpritesDict[sprites[ii].name] = sprites[ii];

		// Default tiles if not specified		
		corridorWallVariations = CheckDefault("DefaultCorridorWallTexture",corridorWallVariations);
		corridorFloorVariations = CheckDefault("DefaultCorridorFloorTexture",corridorFloorVariations);
		corridorColumnVariations = CheckDefault("DefaultCorridorColumnTexture",corridorColumnVariations);
		
		corridorFloorUVariations = CheckDefault("DefaultCorridorFloorUTexture",corridorFloorUVariations);
		corridorFloorIVariations = CheckDefault("DefaultCorridorFloorITexture",corridorFloorIVariations);
		corridorFloorLVariations = CheckDefault("DefaultCorridorFloorLTexture",corridorFloorLVariations);
		corridorFloorTVariations = CheckDefault("DefaultCorridorFloorTTexture",corridorFloorTVariations);
		corridorFloorXVariations = CheckDefault("DefaultCorridorFloorXTexture",corridorFloorXVariations);
		

		roomWallVariations = CheckDefault("DefaultRoomWallTexture",roomWallVariations);
		roomFloorVariations = CheckDefault("DefaultRoomFloorTexture",roomFloorVariations);
		roomColumnVariations = CheckDefault("DefaultRoomColumnTexture",roomColumnVariations);
		insideRoomColumnVariations = CheckDefault("DefaultRoomColumnInsideTexture",insideRoomColumnVariations);
		
		roomFloorInsideVariations = CheckDefault("DefaultRoomFloorTexture",roomFloorInsideVariations);
		roomFloorCornerVariations = CheckDefault("DefaultRoomFloorCornerTexture",roomFloorCornerVariations);
		roomFloorBorderVariations = CheckDefault("DefaultRoomFloorBorderTexture",roomFloorBorderVariations);

		doorVariations = CheckDefault("DefaultDoorTexture",doorVariations);
//		roomDoorVariations = CheckDefault("DefaultRoomDoorTexture",roomDoorVariations);
		passageColumnVariations = CheckDefault("DefaultDoorColumnTexture",passageColumnVariations);

		
		corridorWallOVariations = CheckDefault("DefaultCorridorWallTexture",corridorWallOVariations);
		corridorWallUVariations = CheckDefault("DefaultCorridorWallUTexture",corridorWallUVariations);
		corridorWallIVariations = CheckDefault("DefaultCorridorWallITexture",corridorWallIVariations);
		corridorWallLVariations = CheckDefault("DefaultCorridorWallLTexture",corridorWallLVariations);
		corridorWallTVariations = CheckDefault("DefaultCorridorWallTTexture",corridorWallTVariations);
		corridorWallXVariations = CheckDefault("DefaultCorridorWallXTexture",corridorWallXVariations);
		
		roomWallOVariations = CheckDefault("DefaultRoomWallTexture",roomWallOVariations);
		roomWallUVariations = CheckDefault("DefaultRoomWallUTexture",roomWallUVariations);
		roomWallIVariations = CheckDefault("DefaultRoomWallITexture",roomWallIVariations);
		roomWallLVariations = CheckDefault("DefaultRoomWallLTexture",roomWallLVariations);
		roomWallTVariations = CheckDefault("DefaultRoomWallTTexture",roomWallTVariations);
		roomWallXVariations = CheckDefault("DefaultRoomWallXTexture",roomWallXVariations);
		
		rockVariations = CheckDefault("DefaultRockTexture",rockVariations);

		perimeterWallVariations = CheckDefault("DefaultPerimeterWallTexture",perimeterWallVariations);
		perimeterColumnVariations = CheckDefault("DefaultPerimeterColumnTexture",perimeterColumnVariations);

		if (!entrancePrefab) entrancePrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultEntrance",typeof(GameObject)) as GameObject;
		if (!exitPrefab) exitPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/DefaultExit",typeof(GameObject)) as GameObject;
        if (!playerPrefab) playerPrefab = Resources.Load("Daedalus_Resources/Shared/Prefabs/Player2D", typeof(GameObject)) as GameObject;


		mapPlaneOrientation = MapPlaneOrientation.XY;


		if (!spritePrefab) {
			Debug.Log ("You must set a spritePrefab in the PhysicalMap!");
			return false;
		}
		return true;
	}

    override protected Sprite GetDefault(string defaultName, VirtualMap.DirectionType orientation = VirtualMap.DirectionType.None)
    {
//		Debug.Log (defaultName);
		DaedalusDebugUtils.Assert(loadedSpritesDict.ContainsKey(defaultName),"Cannot find default texture asset " + defaultName);
		return loadedSpritesDict[defaultName];
	}


    override protected SpriteChoice[] GetVariations(VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation = VirtualMap.DirectionType.None)
    {
		SpriteChoice[] type_choices = null;
		switch(cell_type){
		case VirtualCell.CellType.Fake3D_Corridor_WallAbove:			type_choices = fake3DCorridorWallAbove;				break;
		case VirtualCell.CellType.Fake3D_Corridor_WallFront:			type_choices = fake3DCorridorWallFront;				break;
		case VirtualCell.CellType.Fake3D_Room_WallAbove:				type_choices = fake3DRoomWallAbove;					break;
		case VirtualCell.CellType.Fake3D_Room_WallFront:				type_choices = fake3DRoomWallFront;					break;
//		case VirtualCell.CellType.DoorHorizontalTop:					type_choices = doorsHorizontalTop;					break;
		case VirtualCell.CellType.DoorHorizontalBottom:					type_choices = doorsHorizontalBottom;				break;
        case VirtualCell.CellType.DoorVertical:                         type_choices = doorsVertical; break;
        default:                                                        type_choices = base.GetVariations(cell_type, orientation); break;
		}	
		return type_choices;
	}


}