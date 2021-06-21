using UnityEngine;
using System.Collections;

public class StandardMapInterpreterBehaviour : MapInterpreterBehaviour {

	override public MapInterpreter Generate(){
		StandardMapInterpreter interpreter = new StandardMapInterpreter();
		interpreter.behaviour = this;
		return interpreter;
	}
	
}
