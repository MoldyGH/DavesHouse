using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Meshes3DPhysicalMap : PhysicalMap<Meshes3DPhysicalMapBehaviour>
{
	override public void CreateObject(VirtualMap map, MetricLocation l, VirtualCell.CellType cell_type, VirtualMap.DirectionType orientation){
		GameObject prefab = behaviour.GetPrefab(cell_type);
		DaedalusDebugUtils.Assert(prefab != null, "No variation was chosen for " + cell_type);
		 
//		Debug.Log (cell_type);
		GameObject go = (GameObject)GameObject.Instantiate(prefab, new Vector3(l.x*behaviour.tileSize,l.storey*(behaviour.wallHeight+behaviour.storeySeparationHeight),l.y*behaviour.tileSize), Quaternion.identity);
		go.name = cell_type.ToString();

		switch(orientation){
			case VirtualMap.DirectionType.West: 	go.transform.localEulerAngles = new Vector3(0,180,0); break;
			case VirtualMap.DirectionType.North: 	go.transform.localEulerAngles = new Vector3(0,270,0); break;
			case VirtualMap.DirectionType.East: 	break;
			case VirtualMap.DirectionType.South: 	go.transform.localEulerAngles = new Vector3(0,90,0); break;
		}

		// Checking orientation and position for ceiling
		if (cell_type == VirtualCell.CellType.CorridorCeiling || cell_type == VirtualCell.CellType.RoomCeiling) {
			Vector3 tmpPos = go.transform.position;
			tmpPos.y += behaviour.wallHeight;
			go.transform.position = tmpPos;
			
			Vector3 tmpRot = go.transform.localEulerAngles;
			tmpRot.x = 180;
			go.transform.localEulerAngles = tmpRot;
		}

		bool isDynamicCell = GetIsDynamicCell (cell_type);
		AddToMapGameObject(cell_type,go, isDynamicCell, l.storey);

	}

	public bool GetIsDynamicCell(VirtualCell.CellType cell_type){
		return VirtualCell.IsDynamic (cell_type);
	}
	
	override public Vector3 GetStartPosition(){
        int vmapStorey = 0;
        Vector3 worldPos = GetRealWorldPosition(this.GetStartLocation(), vmapStorey);
		return worldPos;
	}
	
	override public Vector3 GetEndPosition(){
		int vmapStorey = this.virtualMaps.Length-1;
        Vector3 worldPos = GetRealWorldPosition(this.GetEndLocation(), vmapStorey);
		return worldPos;
	}



    // Get the position of a cell starting_location in world coordinates, also taking into account the size of the map
    public override Vector3 GetRealWorldPosition(CellLocation l, int storey = 0)
    {
        Vector3 pos = this.GetWorldPosition(l, storey);
        switch (behaviour.mapPlaneOrientation)
        {
            case MapPlaneOrientation.XY:
                pos.x *= behaviour.tileSize;
                pos.y *= behaviour.tileSize;
                pos.z *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                break;
            case MapPlaneOrientation.XZ:
                pos.x *= behaviour.tileSize;
                pos.y *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                pos.z *= behaviour.tileSize;
                break;
            case MapPlaneOrientation.YZ:
                pos.x *= (behaviour.wallHeight + behaviour.storeySeparationHeight);
                pos.y *= behaviour.tileSize;
                pos.z *= behaviour.tileSize;
                break;
        }
        return pos;
    }
	
	
	
}