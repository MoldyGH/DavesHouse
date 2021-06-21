using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntRef))]
public class IntRefDrawer : PropertyDrawer {
	
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {	
		EditorGUI.PropertyField(position, property.FindPropertyRelative("v"),new GUIContent(label));
	}
}
