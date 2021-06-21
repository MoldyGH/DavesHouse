using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor (typeof(Prefabs2DPhysicalMapBehaviour))] 
public class Prefabs2DPhysicalMapBehaviourInterface : PhysicalMapBehaviourInterface {
	
	SerializedObject m_Object;
	
    void OnEnable () {
        m_Object = new SerializedObject (target);
	}
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();	

		Prefabs2DPhysicalMapBehaviour t = (Prefabs2DPhysicalMapBehaviour)target;
		t.manualTileSizes = 	EditorGUILayout.Toggle("Manual Tile Sizes",t.manualTileSizes);
		
		m_Object.Update ();
		if (t.manualTileSizes) {
			EditorGUILayout.PropertyField(m_Object.FindProperty("tileSize"),true);
			EditorGUILayout.PropertyField(m_Object.FindProperty("columnSize"),true);
			EditorGUILayout.PropertyField(m_Object.FindProperty("wallSize"),true);
		}
		
		base.DrawSpecificInspector(m_Object);
		m_Object.ApplyModifiedProperties ();
		
	}
	
}
