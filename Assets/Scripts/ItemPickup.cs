using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class ItemPickup : MonoBehaviour
{
    public GameController gc;
    public int ItemToCollect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
   private void OnTriggerEnter(Collider other)
   {
        if(other.gameObject.name == "Player")
        {
            gc.item = ItemToCollect;
            base.gameObject.SetActive(false);
        }
    }
}
