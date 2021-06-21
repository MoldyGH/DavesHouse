using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    public string playerPrefsName;
    // Start is called before the first frame update
    void Start()
    {
        base.GetComponent<Slider>().value = PlayerPrefs.GetFloat(playerPrefsName);
    }

    public void SetSoundTypeVolume()
    {
        PlayerPrefs.SetFloat(playerPrefsName, base.GetComponent<Slider>().value);
    }
}
