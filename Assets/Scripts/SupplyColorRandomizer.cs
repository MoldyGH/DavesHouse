using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyColorRandomizer : MonoBehaviour
{
    public SpriteRenderer supplySprite;

    // Start is called before the first frame update
    void Start()
    {
        supplySprite = base.GetComponent<SpriteRenderer>();
        supplySprite.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
