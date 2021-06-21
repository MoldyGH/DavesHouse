using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor (typeof(GeneratorBehaviour))]
public class GeneratorBehaviourInterface : Editor {
	
	GUIStyle errorStyle;

	public bool showAdvanced = false;
    public bool showDebugCoordinates = false;
    public bool showDebugConnections = false;
    private GeneratorBehaviour targetInstance;
	public override void OnInspectorGUI() {
		errorStyle = new GUIStyle();
		errorStyle.normal.textColor = Color.red;

		GeneratorBehaviour t = (GeneratorBehaviour) target;
		PhysicalMapBehaviour physicalMapBehaviour = t.gameObject.GetComponent<PhysicalMapBehaviour>();
		MapInterpreterBehaviour interpreterBehaviour = t.gameObject.GetComponent<MapInterpreterBehaviour>();
        VirtualMapGeneratorBehaviour vmapGeneratorBehaviour = t.gameObject.GetComponent<VirtualMapGeneratorBehaviour>();

        targetInstance = t;

		t.mapDimensionsType = (MapDimensionsType)EditorGUILayout.EnumPopup("Map Dimensions Type",t.mapDimensionsType);
		
		t.MapWidth = EditorGUILayout.IntSlider("Map Width",t.MapWidth,2,GeneratorBehaviour.MAX_SIZE);
		t.MapHeight = EditorGUILayout.IntSlider("Map Length",t.MapHeight,2,GeneratorBehaviour.MAX_SIZE);
		
		EditorGUILayout.BeginHorizontal();
		t.useSeed = EditorGUILayout.Toggle("Use seed",t.useSeed);
		EditorGUI.BeginDisabledGroup(t.useSeed == false);
		t.seed = EditorGUILayout.IntSlider("Seed",t.seed,0,10000);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();

		if (t.mapDimensionsType == MapDimensionsType.THREE_DEE){
			t.multiStorey = EditorGUILayout.Toggle("Multi-Storey",t.multiStorey);
			if (t.multiStorey) t.numberOfStoreys = EditorGUILayout.IntSlider("Number of Storeys",t.numberOfStoreys,1,10);
			else t.numberOfStoreys = 1;
		} else {
			t.multiStorey = false;
			t.numberOfStoreys = 1;
		}

		this.showAdvanced = EditorGUILayout.Foldout(this.showAdvanced, "Advanced Options");
		if(this.showAdvanced){
			GUILayout.BeginHorizontal();
			t.printTimings = EditorGUILayout.Toggle("Print Timings",t.printTimings);
			GUILayout.EndHorizontal();
			EditorGUILayout.LabelField("Last used seed: " + t.lastUsedSeed);
            showDebugCoordinates = EditorGUILayout.Toggle("Show Debug Coordinates", showDebugCoordinates);
            showDebugConnections = EditorGUILayout.Toggle("Show Debug Connections", showDebugConnections);
		}

		EditorGUI.BeginDisabledGroup(Application.isPlaying == true);
        bool canGenerate = true;
        if (vmapGeneratorBehaviour == null)
        {
            GUILayout.Label("Attach a Virtual Map Generator Behaviour to enable generation!", errorStyle);
            canGenerate = false;
        }
		if (physicalMapBehaviour == null){
			GUILayout.Label("Attach a Physical Map Behaviour to enable generation!",errorStyle);	
			canGenerate = false;
		}
		else if (t.mapDimensionsType == MapDimensionsType.THREE_DEE && !physicalMapBehaviour.SupportsThreeDeeMap){
			GUILayout.Label("The attached Physical Map Behaviour does not support a 3D map!",errorStyle);	
			canGenerate = false;
		}
		if (interpreterBehaviour == null){
			GUILayout.Label("Attach a Map Interpreter Behaviour to enable generation!",errorStyle);	
			canGenerate = false;
        }

		EditorGUI.BeginDisabledGroup(!canGenerate);
		if (GUILayout.Button("Generate!")) t.Generate();
		EditorGUI.EndDisabledGroup();
		
		#region Save/Load buttons
		EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save")) SavePrefab();
		EditorGUILayout.EndHorizontal();
		#endregion
		EditorGUI.EndDisabledGroup();
		
	}
	
	private void SavePrefab(){
		GameObject prefab = ((GeneratorBehaviour) target).gameObject;
		
		string path = EditorUtility.SaveFilePanel("Save file...", "Assets", "DungeonPrefab", "prefab");
		string[] path_tokens = path.Split('/');
		for (int i=0; i<path_tokens.Length; i++){
			if (path_tokens[i] == "Assets"){
				path = "Assets";
				for (int j=i+1; j<path_tokens.Length; j++){
					path += "/"+path_tokens[j];
				}
				break;
			}
		}
		
		if (path == "") return;
		
		PrefabUtility.CreatePrefab(path,prefab);
		
	}

    void OnSceneGUI()
    {
        if (showDebugCoordinates || showDebugConnections)
        {
            PhysicalMap physicalMap = this.targetInstance.GetPhysicalMap();
            VirtualMap[] virtualMaps = this.targetInstance.GetVirtualMaps();
            if (this.targetInstance.HasGeneratedDungeon())
            {
                if (physicalMap != null && virtualMaps.Length > 0)
                {
                    GUIStyle guiStyle = new GUIStyle();

                    if (showDebugCoordinates){
                        for (int i = 0; i < virtualMaps.Length; i++)
                        {
                            foreach (VirtualCell c in virtualMaps[i].GetAllCells())
                            {
                                Vector3 p = physicalMap.GetRealWorldPosition(c.location, i);
                                if (c.location.x % 2 == 1 && c.location.y % 2 == 1)
                                {
                                    guiStyle.normal.textColor = Color.yellow;
                                    int actual_x = (c.location.x-1)/2;
                                    int actual_y = (c.location.y-1)/2;
                                    Handles.Label(p, c.location.x + "," + c.location.y + "\n(" + actual_x + "," + actual_y + ")", guiStyle);
                                }
                                else
                                {
                                    guiStyle.normal.textColor = Color.white;
                                    Handles.Label(p, c.location.x + "," + c.location.y, guiStyle);
                                }
                            }
                        }
                    }

                    if (showDebugConnections){
                        for (int i = 0; i < virtualMaps.Length; i++)
                        {
                            VirtualMap virtualMap = virtualMaps[i];
                            List<CellLocation> unvisited_locations = new List<CellLocation>(virtualMap.WalkableLocations);
                            //Debug.Log("Starting from " + start_location);
                            int max_distance = virtualMap.GetMaximumDistance();
                            
                            int stop_iter = 0;
                            while(unvisited_locations.Count > 0){
                                CellLocation start_location = unvisited_locations[0]; // We start from a walkable cell (doesn't matter which one)
                                VirtualCell start_cell = virtualMap.GetCell(start_location);
                                
                                DrawConnections(start_cell, unvisited_locations, physicalMap, virtualMap, max_distance);

                                stop_iter ++;
                                if (stop_iter == 100) {
                                    DaedalusDebugUtils.Assert(false,"Looping in show debug connectsion!"); 
                                    break; // ERROR HERE!
                                }
                            }
                        }
                    }

                    // TEST path check
                    //VirtualMap test_virtualMap = virtualMaps[0];
                    //List<CellLocation> test_unvisited_locations = new List<CellLocation>(test_virtualMap.WalkableLocations);
                    //test_virtualMap.ExistsPathBetweenLocations(test_unvisited_locations[0], test_unvisited_locations[test_unvisited_locations.Count - 1]);
                }
            }
        }
    }

    void DrawConnections(VirtualCell input_cell, List<CellLocation> unvisited_locations, PhysicalMap physicalMap, VirtualMap virtualMap, int max_distance)
    {
        unvisited_locations.Remove(input_cell.location);
        Vector3 input_pos = physicalMap.GetRealWorldPosition(input_cell.location, virtualMap.storey_number);
        //Debug.Log("Has connected cells: " + start_cell.connectedCells.Count);
        foreach (CellLocation connected_cell_location in input_cell.connectedCells)
        {
            //Debug.Log(connected_cell_location);
            Vector3 end_pos = physicalMap.GetRealWorldPosition(connected_cell_location, virtualMap.storey_number); 
            //Debug.Log(input_cell.distance_from_root * 1f / virtualMap.GetMaximumDistance());
            Color col = Color.Lerp(Color.red, Color.green, input_cell.distance_from_root * 1f / max_distance);
            Handles.color = col;
            Handles.DrawLine(input_pos, end_pos);
            if (unvisited_locations.Contains(connected_cell_location))
            {
                DrawConnections(virtualMap.GetCell(connected_cell_location), unvisited_locations, physicalMap, virtualMap, max_distance);
            }
        }
    }
}
