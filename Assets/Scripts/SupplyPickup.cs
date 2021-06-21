using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyPickup : MonoBehaviour
{
    public GameController gc;
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
            base.gameObject.SetActive(false);
            this.gc.supplies++;
            this.gc.UpdateSupplyCount();
        }
   }
}
