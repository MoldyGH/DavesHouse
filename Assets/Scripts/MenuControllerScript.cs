using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MenuControllerScript : MonoBehaviour
{
    public AudioMixer audioMixer;

    Resolution[] resolutions;

    public Dropdown resolutionDropdown;

    public AudioClip intro;
    
    public AudioClip house;

    public AudioClip fog;

    public AudioSource musicPlayer;

    public Slider mazeSizeSlider;

    public TMP_Text mazeSizeNumText;

    public float mazeSizeValue;

    public CursorControllerScript cc;

    Slider[] sliders = new Slider[0];
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }

    // Update is called once per frame
    void Update()
    {
        mazeSizeValue = mazeSizeSlider.value;
        mazeSizeNumText.text = mazeSizeValue.ToString();
        PlayerPrefs.SetFloat("MazeSize", this.mazeSizeSlider.value);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetMusic(int selection)
    {
        if(selection == 0)
        {
            musicPlayer.clip = intro;
            musicPlayer.Play();
        }
        if (selection == 1)
        {
            musicPlayer.clip = house;
            musicPlayer.Play();
        }
        if (selection == 2)
        {
            musicPlayer.clip = fog;
            musicPlayer.Play();
        }
    }
    public void Mute(bool isMuted)
    {
        if(isMuted)
        {
            musicPlayer.mute = true;
        }
        if(!isMuted)
        {
            musicPlayer.mute = false;
        }
    }
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteAll();
        sliders = GameObject.FindObjectsOfType<Slider>();
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].value = 0;
        }
        SetFullscreen(true);
    }
}
