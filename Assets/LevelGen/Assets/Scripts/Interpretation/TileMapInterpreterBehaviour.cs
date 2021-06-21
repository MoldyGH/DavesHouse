using UnityEngine;
using System.Collections;

public class TileMapInterpreterBehaviour : MapInterpreterBehaviour {
	
	public bool fillWithFloors = false;
	public bool useFake3DWalls = false;
	//	public bool fillWithCeilings = false;	// This is a consequence of fillWithFloors anyway

	public bool orientPerimeter = false;

	override public MapInterpreter Generate(){
		TileMapInterpreter interpreter = new TileMapInterpreter();
		interpreter.behaviour = this;
		return interpreter;
	}
}
