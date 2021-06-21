using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeScript : MonoBehaviour
{
    public GameController gc;

    private Collider playerCol;
    // Start is called before the first frame update
    void Start()
    {
        playerCol = GameObject.Find("Player").GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other = playerCol)
        {
            gc.item = 6;
        }     
    }
}
