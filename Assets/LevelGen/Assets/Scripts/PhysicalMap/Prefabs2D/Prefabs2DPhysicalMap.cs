using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prefabs2DPhysicalMap : PhysicalMap<Prefabs2DPhysicalMapBehaviour>
{
	override public void CreateObject(VirtualMap map, MetricLocation l, VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation){
		GameObject prefab = behaviour.GetPrefab(cell_type);
		DaedalusDebugUtils.Assert(prefab != null, "No variation was chosen for " + cell_type);
		 
		GameObject go = (GameObject)GameObject.Instantiate(prefab, new Vector3(l.x*behaviour.tileSize,0,l.y*behaviour.tileSize), Quaternion.identity);
		go.name = cell_type.ToString();

		if (orientation == VirtualMap.DirectionType.None) orientation = VirtualMap.DirectionType.North;
		switch(orientation){
			case VirtualMap.DirectionType.West: 	go.transform.localEulerAngles = new Vector3(90,0,0);  break;
			case VirtualMap.DirectionType.North: 	go.transform.localEulerAngles = new Vector3(90,90,0); break;
			case VirtualMap.DirectionType.East: 	go.transform.localEulerAngles = new Vector3(90,180,0); break;
			case VirtualMap.DirectionType.South: 	go.transform.localEulerAngles = new Vector3(90,270,0); break;
		}

		AddToMapGameObject(cell_type,go, cell_type == VirtualCell.CellType.Door, l.storey);
		
		// Move walls up a bit
		if (VirtualCell.IsFloor(cell_type)){
			// Already good
		} else {
			go.transform.localPosition += Vector3.up*0.01f;
		}
	}
	
	override public Vector3 GetStartPosition(){
		int vmapStorey = 0;
		Vector3 worldPos = GetWorldPosition(this.GetStartLocation(),vmapStorey);

		worldPos.x *=  behaviour.tileSize;
		worldPos.y *=  behaviour.tileSize;
		worldPos.z *= 0;
		return worldPos;
	}
	
	override public Vector3 GetEndPosition(){
		int vmapStorey = this.virtualMaps.Length-1;
		Vector3 worldPos = GetWorldPosition(this.GetEndLocation(),vmapStorey);

		worldPos.x *=  behaviour.tileSize;
		worldPos.y *=  behaviour.tileSize;
		worldPos.z *= 0;
		return worldPos;
	}
	
}