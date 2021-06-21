using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerLockScript : MonoBehaviour
{
    public GameController gc;

    public KeyCode mouseButtonKey;

    public GameObject padlock;

    public Collider playerCol;
    // Start is called before the first frame update
    void Start()
    {
        playerCol = GameObject.Find("Player").GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
            
    }
    private void OnTriggerEnter(Collider player)
    {
        if(player = playerCol)
        {
            if (gc.item == 6)
            {
                padlock.SetActive(false);
                base.gameObject.SetActive(false);
                gc.item = 0;
            }
        }
    }
}
