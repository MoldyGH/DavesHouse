using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DiggingVirtualMapGeneratorBehaviour))]
public class DiggingVirtualMapGeneratorBehaviourInterface : Editor
{
    public override void OnInspectorGUI()
    {
        DiggingVirtualMapGeneratorBehaviour t = (DiggingVirtualMapGeneratorBehaviour)target;
        MapInterpreterBehaviour interpreterBehaviour = t.gameObject.GetComponent<MapInterpreterBehaviour>();

        // Entrance/Exit (COMMON)
        EditorGUILayout.BeginHorizontal();
        t.createStartAndEnd = EditorGUILayout.Toggle("Create Entrance and Exit", t.createStartAndEnd);
        if (t.HasRooms && t.createStartAndEnd) t.forceStartAndEndInRooms = EditorGUILayout.Toggle("Force in Room", t.forceStartAndEndInRooms);
        EditorGUILayout.EndHorizontal();
        if (t.createStartAndEnd) EditorGUILayout.MinMaxSlider(new GUIContent("Min & Max distance % between entrance and exit"),ref t.minimumDistanceBetweenStartAndEnd,ref t.maximumDistanceBetweenStartAndEnd,0,100);


        t.algorithmChoice = (DiggingMapGeneratorAlgorithmChoice)EditorGUILayout.EnumPopup("Generation Algorithm: ", t.algorithmChoice);
		
        t.directionChangeModifier = EditorGUILayout.IntSlider(new GUIContent("Non-Linearity %", "The higher this is, the more winding the dungeon will be."), t.directionChangeModifier, 0, 100);
        t.sparsenessModifier = EditorGUILayout.IntSlider(new GUIContent("Sparseness %", "The higher this is, the less branching and the shorter the dungeon will be."), t.sparsenessModifier, 0, 90);	// Sparseness cannot be too high!
        t.openDeadEndModifier = EditorGUILayout.IntSlider(new GUIContent("Link Dead Ends %", "The higher this is, the more the dungeon will not end abruptly."), t.openDeadEndModifier, 0, 100);

        if (interpreterBehaviour != null && interpreterBehaviour.SupportsRooms)
        {
            t.createRooms = EditorGUILayout.Toggle("Create Rooms", t.createRooms);
            if (t.createRooms)
            {
                EditorGUILayout.BeginHorizontal();
                t.minRooms = EditorGUILayout.IntSlider("Min Rooms", t.minRooms, 0, 10);
                t.maxRooms = EditorGUILayout.IntSlider("Max Rooms", t.maxRooms, 0, 10);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                t.minRoomWidth = EditorGUILayout.IntSlider("Min Room Width", t.minRoomWidth, 2, 10);
                t.maxRoomWidth = EditorGUILayout.IntSlider("Max Room Width", t.maxRoomWidth, 2, 10);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                t.minRoomHeight = EditorGUILayout.IntSlider("Min Room Length", t.minRoomHeight, 2, 10);
                t.maxRoomHeight = EditorGUILayout.IntSlider("Max Room Length", t.maxRoomHeight, 2, 10);
                EditorGUILayout.EndHorizontal();

                t.doorsDensityModifier = EditorGUILayout.IntSlider("Passage Density %", t.doorsDensityModifier, 0, 100);
                t.forceRoomTransversal = EditorGUILayout.Toggle(new GUIContent("Force Room Transversal", "If enabled, rooms will be placed such as to force their transversal from one end to the other of the dungeon."), t.forceRoomTransversal);
            }
        }
        else
        {
            t.createRooms = false;
        }
    }
}