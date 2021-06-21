using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor (typeof(MapInterpreterBehaviour))]
public class MapInterpreterBehaviourInterface : Editor {
	
	public GeneratorBehaviour g;
    public PhysicalMapBehaviour p;
    public VirtualMapGeneratorBehaviour v;
	public override void OnInspectorGUI() {		
		MapInterpreterBehaviour t = (MapInterpreterBehaviour)target;
		if (g == null) g = t.gameObject.GetComponent<GeneratorBehaviour>();
        if (p == null) p = t.gameObject.GetComponent<PhysicalMapBehaviour>();
        if (v == null) v = t.gameObject.GetComponent<VirtualMapGeneratorBehaviour>();
		
		t.drawRocks = 			EditorGUILayout.Toggle("Draw Rocks",t.drawRocks);
		t.drawWallCorners = 	EditorGUILayout.Toggle("Draw Wall Corners",t.drawWallCorners);
		t.drawDoors = 			EditorGUILayout.Toggle("Place Doors in Passages",t.drawDoors);

		if (v != null && v.HasRooms)
            t.createColumnsInRooms = EditorGUILayout.Toggle("Draw Columns in Rooms",t.createColumnsInRooms);

		t.useDirectionalFloors = 	EditorGUILayout.Toggle("Use Directional Floors",t.useDirectionalFloors);

		if (t.useDirectionalFloors) t.randomOrientations = false;
		else t.randomOrientations = EditorGUILayout.Toggle("Randomize Orientations",t.randomOrientations);
		
		if (t.useDirectionalFloors) t.isolateDirectionalWallsForRooms = EditorGUILayout.Toggle(new GUIContent("Isolate Rooms","If enabled, room corners will be isolated from the rest of the dungeon."),t.isolateDirectionalWallsForRooms);

		t.usePerimeter =EditorGUILayout.Toggle("Use Perimeter Walls",t.usePerimeter);
//		if (t.usePerimeter) t.internalPerimeter =EditorGUILayout.Toggle("Internal Perimeter",t.internalPerimeter);

		if (g.mapDimensionsType == MapDimensionsType.THREE_DEE){
			t.addCeilingToCorridors = EditorGUILayout.Toggle("Add Ceiling to Corridors",t.addCeilingToCorridors);
            if (v.HasRooms)
                t.addCeilingToRooms = 	EditorGUILayout.Toggle("Add Ceiling To Rooms",t.addCeilingToRooms);
		}
	}

}
