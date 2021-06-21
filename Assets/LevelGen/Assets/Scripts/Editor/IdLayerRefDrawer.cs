using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(IdLayerRef))]
public class IdLayerRefDrawer : PropertyDrawer {
	
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {	
		// An error is given due to: http://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
		EditorGUILayout.BeginHorizontal();
		position.width = position.width/2;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("id"),new GUIContent("Id"));
		position.x += position.width;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("layer"),new GUIContent("Layer"));
		EditorGUILayout.EndHorizontal();
	}
}
