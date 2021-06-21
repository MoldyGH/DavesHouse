using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor(typeof(PrefabOrientedSectionsPhysicalMapBehaviour))] 
public class PrefabOrientedSectionsPhysicalMapBehaviourInterface : PhysicalMapBehaviourInterface {
	
	SerializedObject m_Object;
	
    void OnEnable () {
        m_Object = new SerializedObject (target);
	}
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

        PrefabOrientedSectionsPhysicalMapBehaviour t = (PrefabOrientedSectionsPhysicalMapBehaviour)target;
		t.manualTileSizes = 	EditorGUILayout.Toggle("Manual Section Size",t.manualTileSizes);
		
		m_Object.Update ();
		if (t.manualTileSizes) {
			EditorGUILayout.PropertyField(m_Object.FindProperty("tileSize"),true);
		}

        if (g.multiStorey) t.storeySeparationHeight = EditorGUILayout.FloatField("Storey separation height", t.storeySeparationHeight);

        // We use a different inspector for this one, using just rooms
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionUVariations_N"), new GUIContent("Section U Variations (North)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionUVariations_E"), new GUIContent("Section U Variations (East)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionUVariations_W"), new GUIContent("Section U Variations (West)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionUVariations_S"), new GUIContent("Section U Variations (South)"), true);

        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionIVariations_WE"), new GUIContent("Section I Variations (West-East)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionIVariations_NS"), new GUIContent("Section I Variations (North-South)"), true);

        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionLVariations_NE"), new GUIContent("Section L Variations (North-East)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionLVariations_NW"), new GUIContent("Section L Variations (North-West)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionLVariations_SE"), new GUIContent("Section L Variations (South-East)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionLVariations_SW"), new GUIContent("Section L Variations (South-West)"), true);

        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionTVariations_XE"), new GUIContent("Section T Variations (All-but-East)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionTVariations_XS"), new GUIContent("Section T Variations (All-but-South)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionTVariations_XW"), new GUIContent("Section T Variations (All-but-West)"), true);
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionTVariations_XN"), new GUIContent("Section T Variations (All-but-North)"), true);
        
        EditorGUILayout.PropertyField(m_Object.FindProperty("sectionXVariations"), new GUIContent("Section X Variations"), true);
        //EditorGUILayout.PropertyField(m_Object.FindProperty("sectionLadderVariations"), new GUIContent("Section Ladder Variations"), true);

        // WARNING: this is repeated from DrawSpecificInspector!!
        //if (i != null && i.drawRocks) EditorGUILayout.PropertyField(m_Object.FindProperty("rockVariations"), true);

        if (t.createEntranceAndExit)
        {
            EditorGUILayout.PropertyField(m_Object.FindProperty("entrancePrefab"), true);
            EditorGUILayout.PropertyField(m_Object.FindProperty("exitPrefab"), true);
        }

        // We do not draw the default one, because we have different tiles!
        //base.DrawSpecificInspector(m_Object);
		m_Object.ApplyModifiedProperties ();
		
	}
	
}
