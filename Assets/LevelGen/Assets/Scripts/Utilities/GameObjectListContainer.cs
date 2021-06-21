using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This container is needed for serialization to work correctly
[System.Serializable]
public class GameObjectListContainer
{
	public List<GameObject> _inner_list = new List<GameObject>();
	
	public void Add(GameObject g){
		_inner_list.Add(g);
	}
	
	public int Count{
		get{ return _inner_list.Count;}
	}
	
	public object this[int i]
	{
	    get { return _inner_list[i]; }
	    set { _inner_list[i] = (GameObject)value; }
	}
}