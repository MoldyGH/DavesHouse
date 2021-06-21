using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
	public TMP_Text timerText;
	
	public float time;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        this.time = time += 1f * Time.deltaTime;
        this.timerText.text = this.time.ToString("0");
    }
}