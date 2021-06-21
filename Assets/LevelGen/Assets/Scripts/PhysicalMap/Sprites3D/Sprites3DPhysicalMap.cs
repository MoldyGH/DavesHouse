using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sprites3DPhysicalMap : PhysicalMap<Sprites3DPhysicalMapBehaviour>
{
	
	// Used to avoid leaks
	public List<Material> createdMaterialsList;

	override public void CreateObject(VirtualMap map, MetricLocation l, VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation){
		Texture2D tile_texture = null;
		
		tile_texture = behaviour.GetPrefab(cell_type);

		GameObject go = (GameObject)GameObject.Instantiate(behaviour.tilePrefab, new Vector3(l.x*behaviour.tileSize,0,l.y*behaviour.tileSize), Quaternion.identity);
		
		var tempMaterial = new Material(go.transform.GetComponent<Renderer>().sharedMaterial);
		tempMaterial.SetTexture("_MainTex",tile_texture);
		tempMaterial.name = cell_type.ToString()+ "_Material";
		go.transform.GetComponent<Renderer>().sharedMaterial = tempMaterial;

		if (createdMaterialsList == null) createdMaterialsList = new List<Material>();
		createdMaterialsList.Add(tempMaterial);
		
		go.name = cell_type.ToString();
		AddToMapGameObject(cell_type,go);

		
		go.transform.localEulerAngles = new Vector3(0,0,0);
		switch(orientation){
		case VirtualMap.DirectionType.West: 	go.transform.localEulerAngles = new Vector3(0,0,0);  break;
		case VirtualMap.DirectionType.North: 	go.transform.localEulerAngles = new Vector3(0,90,0); break;
		case VirtualMap.DirectionType.East: 	go.transform.localEulerAngles = new Vector3(0,180,0); break;
		case VirtualMap.DirectionType.South: 	go.transform.localEulerAngles = new Vector3(0,270,0); break;
		}

		// Move walls up a bit
		if (VirtualCell.IsFloor(cell_type)){
			// Already good
		} else {
			go.transform.localPosition += Vector3.up*0.01f;
		}
	}
	
	override public Vector3 GetStartPosition(){
		return GetWorldPosition(this.GetStartLocation())*behaviour.tileSize;
	}
	
	override public Vector3 GetEndPosition(){
		return GetWorldPosition(this.GetEndLocation())*behaviour.tileSize;
	}


	override public void CleanUp(){
		// Delete the created materials
		if (createdMaterialsList != null){
			foreach(Material m in createdMaterialsList){
				DestroyImmediate(m);
			}
			createdMaterialsList.Clear();
		}
		base.CleanUp();
	}
	
}



// TODO: This creates many draw calls because it copies many materials! Fix that with material sharing!
// Here is some useful code for that:


// We cannot just create materials, because then we cannot use prefab choices!
// We should instead link each prefab choice to a material!
//	private List<Material> materials;

//	override public void ResetToWalls (VirtualMap map, GeneratorBehaviour g, PhysicalMapBehaviour b)
//	{
//		base.ResetToWalls(map,g,b);
//		this.behaviour = (TileMap3DSpritesBehaviour)b;
//		
////		this.tilePrefab = generator.corridorFloorPrefab;
////		createMaterial("wall",generator.wallTile);s
//		
////		materials = new List<Material>();
////		foreach(TileType tile_type in System.Enum.GetValues(typeof(TileType))){
////			Material mat = new Material();
////			mat.SetTexture("_MainTex",GetTextu
////		}
//	}
//	
//	private void createMaterial(Texture2D texture){
//		Material mat = new Material(source_material);
//		mat.SetTexture("_MainTex",texture);
//		this.materials[
//	}
