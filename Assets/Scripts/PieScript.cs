using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PieScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        this.rb = base.GetComponent<Rigidbody>();
        this.rb.velocity = base.transform.forward * this.speed;
    }

    // Token: 0x0600002F RID: 47 RVA: 0x00002D88 File Offset: 0x00001188
    private void Update()
    {
        this.rb.velocity = base.transform.forward * this.speed;
        if(davehasbeenpied == true)
        {
            time -= Time.deltaTime;
        }
        if(time <= 0 & davehasbeenpied == true)
        {
            dave.SetActive(true);
            davehasbeenpied = false;
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.name == "davecollider")
        {
            dave.SetActive(false);
            this.time = 15f;
            davehasbeenpied = true;
        }
    }

    // Token: 0x0400003A RID: 58
    public float speed;

    // Token: 0x0400003C RID: 60
    private Rigidbody rb;

    public GameObject dave;

    public float time;

    public bool davehasbeenpied;
}
