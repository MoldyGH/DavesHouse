using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor (typeof(TileMapInterpreterBehaviour))]
public class TileMapInterpreterBehaviourInterface : MapInterpreterBehaviourInterface {
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		TileMapInterpreterBehaviour t = (TileMapInterpreterBehaviour)target;
		t.fillWithFloors = 	EditorGUILayout.Toggle("Fill With Floors",t.fillWithFloors);

		// TODO: Make Fake-3d-Walls not dependent on Sprites2D
		bool sprites2d = p is Sprites2DPhysicalMapBehaviour;
		bool canUseFake3DWalls = sprites2d; // && !t.useDirectionalFloors;
//		if (!canUseFake3DWalls) t.useFake3DWalls = false;
		EditorGUI.BeginDisabledGroup(!canUseFake3DWalls);
		t.useFake3DWalls = EditorGUILayout.Toggle("Fake 3D Walls",t.useFake3DWalls);
		EditorGUI.EndDisabledGroup(); 

		if (t.usePerimeter) t.orientPerimeter = EditorGUILayout.Toggle("Orient Perimeter",t.orientPerimeter);
	}

}
