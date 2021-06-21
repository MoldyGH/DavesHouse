using UnityEngine;
using System.Collections;

public class OpenDoorsAnimationTrigger : AbstractOpenDoors {
	public Animation doors_animation;
	public float animationSpeed = 1.0f;
	
	override protected void AnimateOpen(){
		doors_animation["DoorOpen"].speed = animationSpeed;;
		if (!doors_animation.isPlaying) doors_animation.Play("DoorOpen");
	}
	
	override protected void AnimateClose(){
		doors_animation["DoorOpen"].speed = -animationSpeed;;
		if (!doors_animation.isPlaying) {
			doors_animation["DoorOpen"].time = doors_animation["DoorOpen"].length;
			doors_animation.Play("DoorOpen");
		}
	}

}
