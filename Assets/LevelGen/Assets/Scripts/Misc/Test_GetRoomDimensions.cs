using UnityEngine;
using System.Collections;

public class Test_GetRoomDimensions : MonoBehaviour {

    public GeneratorBehaviour generator;

	void Start () {
        PhysicalMap physicalMap = generator.physicalMap;
        if (physicalMap == null) return;
		
        string s = "Room dimensions:";
        int i = 0;
        foreach (VirtualRoom.Dimensions dim in physicalMap.GetAllRoomDimensions())
        {
            s += "\nRoom " + i + " X: " + dim.start_x + " Y: " + dim.start_y + " DIM X: " + dim.width_x + " DIM Y: " + dim.width_y + " STOREY: " + dim.storey;
            i++;
        }
        Debug.Log(s);
	}
	
}
