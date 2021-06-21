using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BSPVirtualMapGeneratorBehaviour))]
public class BSPVirtualMapGeneratorBehaviourInterface : Editor
{
    public override void OnInspectorGUI()
    {
        BSPVirtualMapGeneratorBehaviour t = (BSPVirtualMapGeneratorBehaviour)target;
        GeneratorBehaviour gen = t.gameObject.GetComponent<GeneratorBehaviour>();
        int maxSplits = 2;
        if (gen != null)
        {
            maxSplits = Mathf.Min(gen.MapHeight, gen.MapWidth) / 4; // No more than these splits or it will behave strangely!
        }

        // Entrance/Exit (COMMON)
        EditorGUILayout.BeginHorizontal();
        t.createStartAndEnd = EditorGUILayout.Toggle("Create Entrance and Exit", t.createStartAndEnd);
        if (t.HasRooms && t.createStartAndEnd) t.forceStartAndEndInRooms = EditorGUILayout.Toggle("Force in Room", t.forceStartAndEndInRooms);
        EditorGUILayout.EndHorizontal();
        if (t.createStartAndEnd) EditorGUILayout.MinMaxSlider(new GUIContent("Min & Max distance % between entrance and exit"), ref t.minimumDistanceBetweenStartAndEnd, ref t.maximumDistanceBetweenStartAndEnd, 0, 100);



        t.nSplits = EditorGUILayout.IntSlider(new GUIContent("Number of splits", "The higher this is, the more and smaller rooms you get."), t.nSplits, 0, maxSplits);
        t.splitRange = EditorGUILayout.Slider(new GUIContent("Split range", "Max range for splitting dungeon sections"), t.splitRange, 0f, 0.5f);
        EditorGUILayout.MinMaxSlider(new GUIContent("Room size range", "Min & Max range for room size"), ref t.roomSizeMinRange, ref t.roomSizeMaxRange, 0f, 1f);
        t.maxCorridorWidth = EditorGUILayout.IntSlider(new GUIContent("Max corridor width", "Large corridors are sometimes added."), t.maxCorridorWidth, 1, 4);
    }
}