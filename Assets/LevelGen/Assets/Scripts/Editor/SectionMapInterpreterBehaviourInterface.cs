using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[CustomEditor (typeof(SectionMapInterpreterBehaviour))]
public class SectionMapInterpreterBehaviourInterface :MapInterpreterBehaviourInterface {
	
	public override void OnInspectorGUI() {
        // No GUI for this one!
		//base.OnInspectorGUI();
       // SectionMapInterpreterBehaviour t = (SectionMapInterpreterBehaviour)target;
	}
}
