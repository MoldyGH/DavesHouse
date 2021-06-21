using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TapePlayer : MonoBehaviour
{
    public GameController gc;

    public int selectedClip;

    public VideoPlayer screenPlayer;

    public MeshRenderer screen;

    public Material videoScreen;

    public VideoClip[] clips = new VideoClip[3];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play()
    {
        if (gc.item == 3)
        {
            RandomizeClip();
            screen.material = videoScreen;
            gc.item = 0;
        }
    }
    public void RandomizeClip()
    {
        selectedClip = UnityEngine.Random.Range(1, clips.Length);
        screenPlayer.clip = clips[selectedClip];
    }
}
