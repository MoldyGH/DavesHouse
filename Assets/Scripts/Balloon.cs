
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Balloon : MonoBehaviour
{
    public int numberToGet;
    public Transform[] wanderPoints = new Transform[5];
    public GameObject CursorReticle;
    float ColorChangeTime;
    // Update is called once per frame
    private void Update()
    {
        if(base.GetComponent<NavMeshAgent>().velocity.magnitude <= 0)
        {
            int randomNum = UnityEngine.Random.Range(0, wanderPoints.Length);
            base.GetComponent<NavMeshAgent>().SetDestination(wanderPoints[randomNum].position);
        }
        if(ColorChangeTime > 0)
        {
            ColorChangeTime -= Time.deltaTime;
        }
        if(ColorChangeTime <= 0)
        {
            base.GetComponentInChildren<TextMeshPro>().color = Color.white;
        }
    }
    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.GetComponent<Balloon>() & Vector3.Distance(GameObject.FindObjectOfType<PlayerScript>().transform.position, base.transform.position) <= 10.5f)
                {
                    GameObject.FindObjectOfType<PlayerScript>().MathMachineNumber = raycastHit.collider.GetComponent<Balloon>().numberToGet;
                    ColorChangeTime = 1;
                    base.GetComponentInChildren<TextMeshPro>().color = Color.red;
                }
            }
        }
        RaycastHit raycastHit2;
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray2, out raycastHit2))
        {
            if(raycastHit2.collider.GetComponent<Balloon>() || raycastHit2.collider.GetComponent<MathTV>())
            {
                if (Vector3.Distance(GameObject.FindObjectOfType<PlayerScript>().transform.position, raycastHit2.collider.transform.position) <= 10.5f)
                {
                    CursorReticle.SetActive(true);
                }
                else
                {
                    CursorReticle.SetActive(false);
                }
            }
            else
            {
                CursorReticle.SetActive(false);
            }
        }
    }
}
