using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrassMazeManager : MonoBehaviour
{
    public CursorControllerScript cc;

    public GeneratorBehaviour gb;

    public float mapSize;
    // Start is called before the first frame update
    void Start()
    {
        cc.LockCursor();
        mapSize = PlayerPrefs.GetFloat("MazeSize");
        gb.MapWidth = (int)mapSize;
        gb.MapHeight = (int)mapSize;
        gb.Generate();
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
