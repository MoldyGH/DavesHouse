using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;


public class GeneratorDebuggerWindow : EditorWindow {

	public GeneratorBehaviour generator;
	public GeneratorDebugger debugger;
	
	[MenuItem ("Window/Generator Debugger")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        GeneratorDebuggerWindow window = (GeneratorDebuggerWindow) EditorWindow.GetWindow (typeof (GeneratorDebuggerWindow));
		window.position = window.position; // Random stuff just so we do not have the warning that window is never used.
	
	}
	
    
    void OnGUI () {
		this.generator = EditorGUILayout.ObjectField(this.generator,typeof(GeneratorBehaviour),true) as GeneratorBehaviour;

		if (debugger == null) debugger = new GeneratorDebugger();

		if (generator != null){
			debugger.generator = generator; 
			
//			if (debugger == null) {
//				GUIStyle style = new GUIStyle();
//				style.normal.textColor = Color.red;
//				GUILayout.Label("Add a Generator Debugger component to the generator you want to debug!",style);	
//				return;
//			}

			EditorGUI.BeginDisabledGroup(Application.isPlaying == true);
			
			PhysicalMap physicalMap = generator.physicalMap;
			
			if (physicalMap == null){
				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.red;
				GUILayout.Label("No map has been generated, generate first!",style);	
			}
			
			EditorGUI.BeginDisabledGroup(physicalMap == null || generator == null);

//			int i = 0;
//			foreach(PhysicalMapZone zone in physicalMap.zones){ 
			for(int i =0; i<physicalMap.zones.Count; i++){
				if (GUILayout.Button("Select Storey " + i)){
					List<GameObject> gos = debugger.SelectObjectsByZone(i);
					if(gos != null) Selection.objects = gos.ToArray();
				}
//				i++;
			}	
			foreach(SelectionObjectType sot in System.Enum.GetValues(typeof(SelectionObjectType)).Cast<SelectionObjectType>()){ 
				if (GUILayout.Button("Select " + sot)){
					List<GameObject> gos = debugger.SelectObjects(sot);
					if(gos != null) Selection.objects = gos.ToArray();
				}
			}		
			if (GUILayout.Button("Clear")) {
				debugger.Clear();	
//				Debug.Log (Selection.objects);
//				Debug.Log (debugger);
				Selection.objects = new Object[0];
			}
			EditorGUI.EndDisabledGroup();


		} else {
			GUILayout.Label("Link your GeneratorBehaviour here.");	
		}

	}
}
