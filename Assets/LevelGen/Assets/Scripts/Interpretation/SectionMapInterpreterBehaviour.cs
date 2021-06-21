using UnityEngine;
using System.Collections;

public class SectionMapInterpreterBehaviour : MapInterpreterBehaviour {
	
    override public bool SupportsRooms { get { return false; } }

	override public MapInterpreter Generate(){
        SectionMapInterpreter interpreter = new SectionMapInterpreter();
		interpreter.behaviour = this;
		return interpreter;
	}
}
