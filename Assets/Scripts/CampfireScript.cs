using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampfireScript : MonoBehaviour
{
    public float campCoinValue;
    // Start is called before the first frame update
    void Start()
    {
        campCoinValue = PlayerPrefs.GetFloat("Coins") + 5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter()
    {
        SceneManager.LoadScene("Menu");
        PlayerPrefs.SetFloat("Coins", campCoinValue);
    }
}
