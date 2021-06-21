using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor (typeof(Sprites3DPhysicalMapBehaviour))]
public class Sprites3DPhysicalMapBehaviourInterface : PhysicalMapBehaviourInterface {
	SerializedObject m_Object;
    void OnEnable () {
        m_Object = new SerializedObject (target);
	}
	
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		Sprites3DPhysicalMapBehaviour t = (Sprites3DPhysicalMapBehaviour)target;
		
		m_Object.Update ();
		
		t.tilePrefab = EditorGUILayout.ObjectField("Tile Prefab",t.tilePrefab,typeof(GameObject),true) as GameObject;	

		base.DrawSpecificInspector(m_Object);

		m_Object.ApplyModifiedProperties ();
	
	}
	
}
