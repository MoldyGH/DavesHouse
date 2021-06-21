using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor (typeof(Meshes3DPhysicalMapBehaviour))]
public class Meshes3DPhysicalMapBehaviourInterface : PhysicalMapBehaviourInterface {
	
	SerializedObject m_Object;
	
    void OnEnable () {
        m_Object = new SerializedObject (target);
	}
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();	
		
		Meshes3DPhysicalMapBehaviour t = (Meshes3DPhysicalMapBehaviour)target;
				
		if (g.multiStorey) t.storeySeparationHeight = 	EditorGUILayout.FloatField("Storey separation height",t.storeySeparationHeight);

		t.manualTileSizes = 	EditorGUILayout.Toggle("Manual Tile Sizes",t.manualTileSizes);

		m_Object.Update ();
		if (t.manualTileSizes) {
			EditorGUILayout.PropertyField(m_Object.FindProperty("tileSize"),true);
			EditorGUILayout.PropertyField(m_Object.FindProperty("columnSize"),true);
			EditorGUILayout.PropertyField(m_Object.FindProperty("wallSize"),true);
		}

		if (i != null && i.addCeilingToCorridors) {
			EditorGUILayout.PropertyField(m_Object.FindProperty("corridorCeilingVariations"),true);
		}

        if (v.HasRooms)
        {
            if (i.addCeilingToRooms)
            {
                EditorGUILayout.PropertyField(m_Object.FindProperty("roomCeilingVariations"), true);
            }
        }

		if (g.multiStorey){
			EditorGUILayout.PropertyField(m_Object.FindProperty("ladderVariations"),true);
			EditorGUILayout.PropertyField(m_Object.FindProperty("ladder2Variations"),true);
		}
		
		base.DrawSpecificInspector(m_Object);
		m_Object.ApplyModifiedProperties ();
		
	}
	
}
