using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Test_GetMapDimensions : MonoBehaviour {

    public GeneratorBehaviour generator;

	void Update () {
        PhysicalMap physicalMap = generator.physicalMap;
        if (physicalMap == null) return;
		
        string s = "Map dimensions:";
        foreach (VirtualMap.Dimensions dim in physicalMap.GetMapDimensions())
        {
            s += "\nStorey " + dim.storey + " L: " + dim.left + " R: " + dim.right + " B: " + dim.bottom + " T: " + dim.top  + " DIM X: " + dim.width_x + " DIM Y: " + dim.width_y;
        }
        Debug.Log(s);
	}
	
}
