using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GeneratorDebugger {
	public static bool OVERRIDE_MATERIALS = false;
	
	private Dictionary<SelectionObjectType,Material> originalMaterials = new Dictionary<SelectionObjectType, Material>();

	public GeneratorBehaviour generator;
	
	private List<GameObject> SelectInternal(PhysicalMap physicalMap, SelectionObjectType type){
		if (physicalMap == null){
			Debug.Log("NO PHYSICAL MAP!");
			return null;
		}

		DaedalusDebugUtils.Assert(physicalMap.gameObjectLists != null, "Physical map's game object lists are null!");
		
		List<GameObject> list = physicalMap.GetObjectsOfType(type) as List<GameObject>;
		if (list.Count == 0) {
			Debug.Log("NO ITEMS!");
			return list;
		}
			
		if (OVERRIDE_MATERIALS){
			Material currentMat = list[0].GetComponentsInChildren<MeshRenderer>()[0].sharedMaterial;
			
			if (originalMaterials.ContainsKey(type) && originalMaterials[type] != null){
				// Already selected!
				return list;
			} else {
				originalMaterials[type] = currentMat;
			}
			
			Material sharedMat = new Material(currentMat);
			sharedMat.color = Color.blue;
			sharedMat.name = "Debug Material";
			foreach(GameObject go in list){
				foreach(MeshRenderer r in go.GetComponentsInChildren<MeshRenderer>()){
					r.sharedMaterial = sharedMat;
				}
			}
		}
		return list;
	}
	
	public List<GameObject> SelectObjects(SelectionObjectType type){
		PhysicalMap physicalMap = generator.physicalMap;
		return SelectInternal(physicalMap, type);
	}

	
	public void Clear(){
		PhysicalMap physicalMap = generator.physicalMap;
		if (physicalMap == null) return;
		
		foreach(SelectionObjectType sot in System.Enum.GetValues(typeof(SelectionObjectType)).Cast<SelectionObjectType>()){ 
			ClearList(physicalMap, sot);
		}
	}
	
	private void ClearList(PhysicalMap physicalMap, SelectionObjectType type){
		List<GameObject> list = physicalMap.GetObjectsOfType(type);
		
		if (OVERRIDE_MATERIALS){
			if (originalMaterials.ContainsKey(type) && originalMaterials[type] != null){
				foreach(GameObject go in list){
					foreach(MeshRenderer r in go.GetComponentsInChildren<MeshRenderer>()){
						r.sharedMaterial = originalMaterials[type];
					}
				}
				originalMaterials[type] = null;
			}
		}
	}





	
	
	public List<GameObject> SelectObjectsByZone(int zone_number){
		PhysicalMap physicalMap = generator.physicalMap;
		if (physicalMap == null){
			Debug.Log("NO PHYSICAL MAP!");
			return null;
		}
		
		DaedalusDebugUtils.Assert(physicalMap.zones != null, "Physical map's zone lists are null!");
		DaedalusDebugUtils.Assert(zone_number >= 0, "Zone number must be >= 0!"); 
		DaedalusDebugUtils.Assert(zone_number <= physicalMap.zones.Count-1, "Zone number too high!"); 
		
		List<GameObject> list = physicalMap.GetObjectsOfZone(zone_number) as List<GameObject>;
		if (list.Count == 0) {
			Debug.Log("NO ITEMS!");
			return list;
		}

		return list;
	}
	

}
