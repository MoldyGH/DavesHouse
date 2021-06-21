using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Transform playerBody;

    private float xAxisClamp;
	
	public GameObject player;
	
	public Vector3 offset;
	
	public Transform dave;
	
	public PlayerScript ps;

    public Transform camera;

    public float warpTimer;

    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0.0f;
        warpTimer = 30f;
    }


    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraRotation();
        this.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
    }
    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        if(xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
	private void LateUpdate()
	{
		base.transform.position = this.player.transform.position + this.offset;
		if (this.ps.gameOver)
		{
			base.transform.position = this.dave.transform.position + this.dave.transform.forward * 2f + new Vector3(0f, 5f, 0f);
		base.transform.LookAt(new Vector3(this.dave.position.x, dave.position.y + 10, this.dave.position.z));
		}
		
	}
}