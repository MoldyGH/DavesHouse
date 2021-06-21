using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CellTypeGameObjectListDict : ScriptableObject {
	[SerializeField]
	public List<VirtualCell.CellType> keys;
	
	[SerializeField]
	public List<GameObjectListContainer> values;
	
	[NonSerialized]
	protected Dictionary<VirtualCell.CellType,List<GameObject>> hidden_dict;
	
	public static CellTypeGameObjectListDict Create(){
		CellTypeGameObjectListDict instance = ScriptableObject.CreateInstance<CellTypeGameObjectListDict>();
		instance.Init();
		return instance;
	}
	
	public void Init(){
		keys = new List<VirtualCell.CellType>();
		values = new List<GameObjectListContainer>();
	}
	
	public void RebuildHiddenDictionary(){
//		Debug.Log("REBUILDING DICTIONARY FROM " + keys.Count + " VALUES!");
		hidden_dict = new Dictionary<VirtualCell.CellType,List<GameObject>>();
		for(int i = 0; i<keys.Count; i++){
			hidden_dict[keys[i]] = values[i]._inner_list;
//			Debug.Log("Set key " + keys[i] + " and value " + values[i] + " with n items: " + values[i].Count);
		}
	}
	
	public List<GameObject> Get(VirtualCell.CellType k){
		if (hidden_dict == null) {
			RebuildHiddenDictionary();
//			Debug.Log("Dictionary keys: " + hidden_dict.Keys.Count);
//			foreach (VirtualCell.CellType c in hidden_dict.Keys){
//				Debug.Log("KEY: " + c);	
//				Debug.Log("VALUE: " + hidden_dict[c]);	
//			}
//			Debug.Log("Chosen list: " + (hidden_dict[k] as List<GameObject>));
		}
		return hidden_dict[k] as List<GameObject>;
	}
	
	public void Set(VirtualCell.CellType k, GameObjectList v){
		if (hidden_dict == null) RebuildHiddenDictionary();
		hidden_dict[k] = v;
		keys.Add(k);
		GameObjectListContainer v_container = new GameObjectListContainer();
		v_container._inner_list = v;
		values.Add(v_container);
	}
}

