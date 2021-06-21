using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tips : MonoBehaviour
{
    public TMP_Text tiptext;

    public string[] tips;
    // Start is called before the first frame update
    void Start()
    {
        int random = Mathf.RoundToInt(UnityEngine.Random.Range(0f, tips.Length - 5));
        tiptext.text = tips[random];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
