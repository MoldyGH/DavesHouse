using UnityEngine;
using System.Collections;

public class OpenDoorsRotateTrigger : AbstractOpenDoors {

	public GameObject animatedGo;
	public Vector3 startRotation;
	public Vector3 endRotation;
	public float animationSpeed = 1.0f;
	
	private int animationDirection = 0;
	
	public void Awake(){
		StartCoroutine("AnimationRoutine");
	}
	
	override protected void AnimateOpen(){
		animationDirection = 1;
	}
	
	override protected void AnimateClose(){
		animationDirection = -1;
	}
	
	
	protected IEnumerator AnimationRoutine(){
		Quaternion startQ = Quaternion.Euler(startRotation);
		Quaternion endQ = Quaternion.Euler(endRotation);
		float t = 0;
		while(true){
			t += Time.deltaTime*animationDirection*animationSpeed;
			t = Mathf.Clamp01(t);
			if (animatedGo != null) animatedGo.transform.localRotation = Quaternion.Slerp(startQ,endQ,t);
			yield return null;
		}
	}
}
