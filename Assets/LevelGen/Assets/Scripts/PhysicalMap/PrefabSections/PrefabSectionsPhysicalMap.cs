using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabSectionsPhysicalMap : PhysicalMap<PrefabSectionsPhysicalMapBehaviour>
{
	override public void CreateObject(VirtualMap map, MetricLocation l, VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation){
		GameObject prefab = behaviour.GetPrefab(cell_type);
		DaedalusDebugUtils.Assert(prefab != null, "No variation was chosen for " + cell_type);

        GameObject go = (GameObject)GameObject.Instantiate(prefab, new Vector3(l.x * behaviour.tileSize, l.storey*(behaviour.wallHeight + behaviour.storeySeparationHeight), l.y * behaviour.tileSize), Quaternion.identity);
		go.name = cell_type.ToString();

		if (orientation == VirtualMap.DirectionType.None) orientation = VirtualMap.DirectionType.North;
		switch(orientation){
			case VirtualMap.DirectionType.West: 	go.transform.localEulerAngles = new Vector3(0,180,0);  break;
			case VirtualMap.DirectionType.North: 	go.transform.localEulerAngles = new Vector3(0,270,0); break;
			case VirtualMap.DirectionType.East: 	go.transform.localEulerAngles = new Vector3(0,0,0); break;
			case VirtualMap.DirectionType.South: 	go.transform.localEulerAngles = new Vector3(0,90,0); break;
		}
		go.transform.localEulerAngles += new Vector3 (0, 180, 0);	// Orientation fix


		AddToMapGameObject(cell_type,go, cell_type == VirtualCell.CellType.Door, l.storey);
	}
	
	override public Vector3 GetStartPosition(){
		int vmapStorey = 0;
		Vector3 worldPos = GetWorldPosition(this.GetStartLocation(),vmapStorey);

        // TODO: this should be part of GetWorldPosition!
        switch (behaviour.mapPlaneOrientation)
        {
            case MapPlaneOrientation.XY:
                worldPos.x *= behaviour.tileSize;
                worldPos.y *= behaviour.tileSize;
                worldPos.z *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                break;
            case MapPlaneOrientation.XZ:
                worldPos.x *= behaviour.tileSize;
                worldPos.y *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                worldPos.z *= behaviour.tileSize;
                break;
            case MapPlaneOrientation.YZ:
                worldPos.x *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                worldPos.y *= behaviour.tileSize;
                worldPos.z *= behaviour.tileSize;
                break;
        }
		return worldPos;
	}
	
	override public Vector3 GetEndPosition(){
		int vmapStorey = this.virtualMaps.Length-1;
		Vector3 worldPos = GetWorldPosition(this.GetEndLocation(),vmapStorey);

        // TODO: this should be part of GetWorldPosition!
        switch (behaviour.mapPlaneOrientation)
        {
            case MapPlaneOrientation.XY:
                worldPos.x *= behaviour.tileSize;
                worldPos.y *= behaviour.tileSize;
                worldPos.z *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                break;
            case MapPlaneOrientation.XZ:
                worldPos.x *= behaviour.tileSize;
                worldPos.y *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                worldPos.z *= behaviour.tileSize;
                break;
            case MapPlaneOrientation.YZ:
                worldPos.x *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                worldPos.y *= behaviour.tileSize;
                worldPos.z *= behaviour.tileSize;
                break;
        }
		return worldPos;
	}
	
}