using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    private GameController gc;

    public float ngCoinValue;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("gc").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" & this.gc.supplies == 8)
        {
            if (gc.mode == "normal")
            {
                ngCoinValue = PlayerPrefs.GetFloat("Coins") + 15f;
                PlayerPrefs.SetFloat("Coins", ngCoinValue);
                SceneManager.LoadScene("Win");
            }
            if (gc.mode == "timed")
            {
                ngCoinValue = PlayerPrefs.GetFloat("Coins") + 30f;
                PlayerPrefs.SetFloat("Coins", ngCoinValue);
                SceneManager.LoadScene("Win");
            }
        }
    }
}
