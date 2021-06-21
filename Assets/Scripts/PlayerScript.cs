using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class PlayerScript : MonoBehaviour {
 
    public float movementSpeed;
	
	public Slider staminaBar;
	
	public float staminaValue;
	
	public float staminaRate;
	
	public int running;
	
	public float maxStamina;
	
	public GameObject UONEEDREST;
	
	public bool gameOver;
	
	public GameController gc;

    public TapePlayer tapePlayerScript;

    public ItemGiver itemGiverScript;

	public int MathMachineNumber;
    // Use this for initialization
    void Start () {
       this.staminaValue = 100f;
	   this.maxStamina = 100f;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
    }
    //Update is called once per frame
    void FixedUpdate () 
	{
		if (!gc.TeleportingPlayer) 
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey("w"))
			{
				transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed * 2.5f;
				this.running = 1;
			}
			else if (Input.GetKey("w") && !Input.GetKey(KeyCode.LeftShift))
			{
				transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed;
				this.running = 0;
			}
			else if (Input.GetKey("s"))
			{
				transform.position -= transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed;
				this.running = 0;
			}

			if (Input.GetKey("a") && !Input.GetKey("d"))
			{
				transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * movementSpeed;
				this.running = 0;
			}
			else if (Input.GetKey("d") && !Input.GetKey("a"))
			{
				transform.position -= transform.TransformDirection(Vector3.left) * Time.deltaTime * movementSpeed;
				this.running = 0;
			}
			if (this.movementSpeed <= 0)
			{
				this.running = 0;
			}
			this.staminaBar.value = this.staminaValue;
			if (this.running == 1 & this.staminaValue > 0)
			{
				this.staminaValue -= this.staminaRate * Time.deltaTime;
			}
			if (this.staminaValue <= 0)
			{
				this.UONEEDREST.SetActive(true);
				this.movementSpeed = 4f;
			}
			if (this.running == 0 & this.staminaValue <= 100)
			{
				this.UONEEDREST.SetActive(false);
				this.staminaValue += this.staminaRate * Time.deltaTime;
			}
			if (this.staminaValue >= 0)
			{
				this.movementSpeed = 8f;
			}
		}
    }
	  private void OnTriggerEnter(Collider other)
	  {
	  	if (other.transform.name == "Dave")
		{
			this.gameOver = true;
			this.gc.jumpscareTime = 2f;
		}
	  }
    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.name == "tapeplayer")
        {
            tapePlayerScript.Play();
        }
    }
}