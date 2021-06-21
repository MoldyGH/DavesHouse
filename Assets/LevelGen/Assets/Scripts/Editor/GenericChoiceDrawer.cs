using UnityEditor;
using UnityEngine;


// See http://catlikecoding.com/unity/tutorials/editor/custom-data/

public class GenericChoiceDrawer : PropertyDrawer {

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
#if UNITY_4_2
		return property.isExpanded && label != GUIContent.none ? 32f : 16f;
#else
		return 16f;
#endif
	}
	
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {		
		position.height = 16f;

#if UNITY_4_2
		if (label != GUIContent.none) {

			Rect foldoutPosition = position;
			
			//			foldoutPosition.x -= 14f;
			//			foldoutPosition.width += 14f;
			label = EditorGUI.BeginProperty(position, label, property);
			property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, label, true);
			EditorGUI.EndProperty();
			
			if (!property.isExpanded) {
				return;
			}
			position.y += 16f;
		}
#endif

		position = EditorGUI.IndentedRect(position);

		float totWidth = position.width;
		position.width = totWidth*2/3f;
		int oldIndentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;


#if UNITY_4_2
		EditorGUIUtility.LookLikeControls(50f);
#else
		EditorGUIUtility.labelWidth = 50f;
#endif 

		// In Unity 4.3-, the properties are called this for each entry in a variations array.
		// In unity 4.5+, the actual array property enters this as well now. It contains an array of 'data' entries, each with their 'choice' and 'weight' 
		//    Maybe, we could use that to personalize the array appearance...
#if UNITY_4_3
#else
//		Debug.Log ("Array?: " + property.isArray);
//		PrintInfoOnProperty(property);
		if (property.isArray){
			return;
		}
#endif

		EditorGUI.PropertyField(position, property.FindPropertyRelative("choice"),new GUIContent("Choice"));
		position.x += position.width;
		position.width = totWidth*1/3f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("weight"),new GUIContent("Weight"));
		EditorGUI.indentLevel = oldIndentLevel;

		// We make sure the weight is at least 1
		SerializedProperty prop = property.FindPropertyRelative("weight");
		if (prop.intValue == 0) prop.intValue = 1;
	}


	public void PrintInfoOnProperty(SerializedProperty prop){
		string s = "";
		s += "Property name: " + prop.name;
		s += "\nChildren:";
		System.Collections.IEnumerator en = prop.GetEnumerator();
		while(en.MoveNext()){
			s += "\n - " + (en.Current as SerializedProperty).name;
			PrintInfoOnProperty(en.Current as SerializedProperty);
		}
		Debug.Log (s);
	}
}
