using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor(typeof(PrefabSectionsPhysicalMapBehaviour))] 
public class PrefabSectionsPhysicalMapBehaviourInterface : PhysicalMapBehaviourInterface {
	
	SerializedObject m_Object;
	
    void OnEnable () {
        m_Object = new SerializedObject (target);
	}
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

        PrefabSectionsPhysicalMapBehaviour t = (PrefabSectionsPhysicalMapBehaviour)target;
		t.manualTileSizes = 	EditorGUILayout.Toggle("Manual Section Size",t.manualTileSizes);
		
		m_Object.Update ();
		if (t.manualTileSizes) {
			EditorGUILayout.PropertyField(m_Object.FindProperty("tileSize"),true);
		}

        if (g.multiStorey) t.storeySeparationHeight = EditorGUILayout.FloatField("Storey separation height", t.storeySeparationHeight);

        // We use a different inspector for this one, using just rooms
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionUVariations"), new GUIContent("Section U Variations"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionIVariations"), new GUIContent("Section I Variations"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionLVariations"), new GUIContent("Section L Variations"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionTVariations"), new GUIContent("Section T Variations"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionXVariations"), new GUIContent("Section X Variations"), true);
        //EditorGUILayout.PropertyField(m_Object.FindProperty("sectionLadderVariations"), new GUIContent("Section Ladder Variations"), true);

        // WARNING: this is repeated from DrawSpecificInspector!!
        if (i != null && i.drawRocks) EditorGUILayout.PropertyField(m_Object.FindProperty("rockVariations"), true);

        if (t.createEntranceAndExit)
        {
            EditorGUILayout.PropertyField(m_Object.FindProperty("entrancePrefab"), true);
            EditorGUILayout.PropertyField(m_Object.FindProperty("exitPrefab"), true);
        }

        //base.DrawSpecificInspector(m_Object);
		m_Object.ApplyModifiedProperties ();
		
	}
	
}
