using UnityEngine;
using System.Collections;

public class OpenDoorsSlideTrigger : AbstractOpenDoors {

	public GameObject animatedGo;
	public Vector3 startPosition;
	public Vector3 endPosition;
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
		float t = 0;
		while(true){
			t += Time.deltaTime*animationDirection*animationSpeed;
			t = Mathf.Clamp01(t);
			if (animatedGo != null) animatedGo.transform.localPosition = Vector3.Lerp(startPosition,endPosition,t);
			yield return null;
		}
	}
}
