using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
    public AudioSource audio;

    public CursorControllerScript cc;
    // Start is called before the first frame update
    void Start()
    {
        cc.UnlockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audio.isPlaying)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
