using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Builds a dungeon out of 2D Sprites
public class Sprites2DPhysicalMap : PhysicalMap<Sprites2DPhysicalMapBehaviour>
{
	List<GameObject> spawnedSpriteGos;

	public bool alreadyCreated;
	private int currentIndex;

	override protected bool StartGeneration(){	

		if (!alreadyCreated){
			spawnedSpriteGos = new List<GameObject>();
			alreadyCreated = false;
			currentIndex = 0;
		}
		return true;
	}
	
	override protected void EndGeneration(){	
		alreadyCreated = true;
	}

	override public void CreateObject(VirtualMap map, MetricLocation l, VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation){
		GameObject go = null;
		if (alreadyCreated && currentIndex < spawnedSpriteGos.Count){
			go = spawnedSpriteGos[currentIndex];
			currentIndex++;
		} else {
			go = (GameObject)GameObject.Instantiate(behaviour.spritePrefab, new Vector3(l.x*behaviour.tileSize,0,l.y*behaviour.tileSize), Quaternion.identity);
			spawnedSpriteGos.Add(go);
		}

		Sprite sprite = behaviour.GetPrefab(cell_type);
		go.GetComponent<SpriteRenderer>().sprite = sprite;

		go.name = cell_type.ToString();
		go.GetComponent<BoxCollider2D>().size = new Vector2(behaviour.tileSize,behaviour.tileSize);
		AddToMapGameObject(cell_type,go);

//		Debug.Log ("Cell at " + l.x+"-"+l.y + " has orientation " + orientation);
		
		go.transform.localEulerAngles = new Vector3(90,0,0);
		switch(orientation){
		case VirtualMap.DirectionType.West: 	go.transform.localEulerAngles = new Vector3(90,0,0);  break;
		case VirtualMap.DirectionType.North: 	go.transform.localEulerAngles = new Vector3(90,90,0); break;
		case VirtualMap.DirectionType.East: 	go.transform.localEulerAngles = new Vector3(90,180,0); break;
		case VirtualMap.DirectionType.South: 	go.transform.localEulerAngles = new Vector3(90,270,0); break;
		}

		
		// Move walls up a bit
		if (VirtualCell.IsFloor(cell_type)){
			// Already good
		} else {
			go.transform.localPosition += Vector3.up*0.01f;
		}

		if (cell_type == VirtualCell.CellType.DoorHorizontalBottom){
			go.transform.localPosition += Vector3.forward*behaviour.tileSize*0.25f;
		}

	}


	override public Vector3 GetStartPosition(){
		return GetWorldPosition(this.GetStartLocation())*behaviour.tileSize;
	}

	override public Vector3 GetEndPosition(){
		return GetWorldPosition(this.GetEndLocation())*behaviour.tileSize;
	}
	

}
