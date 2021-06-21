using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathTV : MonoBehaviour
{
    int Num1;
    int Num2;
    TextMeshPro MathText;
    int solution;
    bool solved;
    public AudioClip SolvedClip;
    public AudioClip NotSolvedClip;
    // Start is called before the first frame update
    void Start()
    {
        MathText = base.GetComponentInChildren<TextMeshPro>();
        Num1 = Random.Range(0, 6);
        Num2 = Random.Range(0, Num1);
        MathText.text = Num1 + " + " + Num2 + " = ?";
        solution = Num1 + Num2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.GetComponent<MathTV>())
                {
                    if (!raycastHit.collider.GetComponent<MathTV>().solved)
                    {
                        if (GameObject.FindObjectOfType<PlayerScript>().MathMachineNumber == raycastHit.collider.GetComponent<MathTV>().solution)
                        {
                            raycastHit.collider.GetComponent<MathTV>().solved = true;
                            raycastHit.collider.GetComponent<MathTV>().MathText.text = raycastHit.collider.GetComponent<MathTV>().Num1 + " + " + raycastHit.collider.GetComponent<MathTV>().Num2 + " = " + raycastHit.collider.GetComponent<MathTV>().solution;                        
                            if (!raycastHit.collider.GetComponent<AudioSource>().isPlaying)
                            {
                                raycastHit.collider.GetComponent<AudioSource>().PlayOneShot(SolvedClip);
                            }
                            PlayerPrefs.SetFloat("Coins", PlayerPrefs.GetFloat("Coins") + 5f);
                        }
                        else
                        {
                            raycastHit.collider.GetComponent<MathTV>().solved = false;
                            if (!raycastHit.collider.GetComponent<AudioSource>().isPlaying)
                            {
                                raycastHit.collider.GetComponent<AudioSource>().PlayOneShot(NotSolvedClip);
                            }
                        }
                    }
                }
            }
        }
    }
}
