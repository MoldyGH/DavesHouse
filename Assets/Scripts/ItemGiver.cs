using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    public Transform itemGiver;

    public float waitTime;

    public AudioSource audioDevice;

    public GameController gc;

    public Transform dave;

    public Collider playerCol;
    // Start is called before the first frame update
    void Start()
    {
        waitTime = UnityEngine.Random.Range(120f, 180f);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
        }
        if (waitTime <= 0)
        {
            Reset();
        }
    }
    public void Reset()
    {
        waitTime = UnityEngine.Random.Range(120f, 180f);
        itemGiver.position = dave.position;
    }
    private void OnTriggerEnter(Collider other)
    {
       if(other = playerCol)
        {
            gc.item = UnityEngine.Random.Range(1, 7);
            Reset();
        }
    }
}
