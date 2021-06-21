using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Diagnostics;

public class GameController : MonoBehaviour
{
	public GameObject pauseMenu;
	
	public bool paused;
	
	public CursorControllerScript cursorController;
	
	public GameObject DaveNormal;
	
	public MenuControllerScript mc;
	
	public Camera gameCamera;
	
	public int supplies;
	
	public TMP_Text supplyText;
	
	private bool timerMode;
	
	public AudioSource normalDaveAudio;
	
	public AudioClip prize;
	
	public GameObject coin;
	
	public int item;
	
	public TMP_Text itemText;
	
	public GameObject itemSprite;
	
	public Sprite nothing;
	
	public Sprite rareCoin;
	
	public GameObject frontDoor;
	
	public AudioSource schoolAudio;
	
	public AudioClip chaseBegin;
	
	public GameObject normalDave;
	
	public AudioClip jumpscare;
	
	public PlayerScript ps;
	
	public AudioSource gameAudio;
	
	public float jumpscareTime;
	
	public DaveScript daveScript;
	
	public GameObject daveObject;

    public Sprite goggles;

    public Material nightSky;

    public Material daySky;

    public GameObject postFX;

    public AudioClip end;

    public Sprite tape;

    public KeyCode input;

    public GameObject goggleOverlay;

    public float goggleTime;

    public bool gogglesAreBeingUsed;

    public TMP_Text distanceFromDave;

    public Transform dave;

    public float daveDistance;

    public Transform player;

    public GameObject itemGiver;

    public Sprite pie;

    public GameObject movingPie;

    public Transform cameraTransform;

    public string mode;

    public TimerScript timerScript;

    public TMP_Text modeText;

    public float timeSinceSceneBegan;

    public Image modeImage;

    public Sprite normalMode;

    public Sprite timedMode;

    public bool chaseMode;

    public Sprite cake;

    public Sprite teleporterItemSPR;

    public PlayerScript playerScript;

    public Sprite hammer;

    public float coinValue;

    public int storeboughtvalue;

    public Transform[] teleportationPositions = new Transform[5];

    public bool TeleportingPlayer;

    public AudioClip PlaceHolderTeleportSound;
    // Start is called before the first frame update
    void Start()
    {
        this.UpdateSupplyCount();
        Cursor.visible = false;
        mode = PlayerPrefs.GetString("mode");
        modeText.text = mode;
        if (mode == "normal")
        {
            modeImage.sprite = normalMode;
        }
        else if (mode == "timed")
        {
            modeImage.sprite = timedMode;
        }
        if(PlayerPrefs.GetInt("ItemPurchase") == 4)
        {
            item = 4;
            PlayerPrefs.SetInt("ItemPurchase", 0);
        }
        if (PlayerPrefs.GetInt("ItemPurchase") == 1)
        {
            item = 1;
            PlayerPrefs.SetInt("ItemPurchase", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {    
        storeboughtvalue = PlayerPrefs.GetInt("ItemPurchase");
        if (Input.GetKeyDown(KeyCode.Escape))
		{
            if (!paused)
            {
                this.pauseMenu.SetActive(true);
                Time.timeScale = 0;
                this.paused = true;
                this.cursorController.UnlockCursor();
            }
            else
            {
                this.pauseMenu.SetActive(false);
                Time.timeScale = 1;
                this.paused = false;
                this.cursorController.LockCursor();
                if (GameObject.Find("Settings").activeSelf)
                {
                    (GameObject.Find("Settings")).SetActive(false);
                }
            }
        }
		if (Input.GetKeyDown(KeyCode.Y) & this.paused)
			{
				SceneManager.LoadScene("Menu");
				this.cursorController.UnlockCursor();
				Time.timeScale = 1;
			}
		else if (Input.GetKeyDown(KeyCode.N))
		{
            if (paused)
            {
                this.pauseMenu.SetActive(false);
                Time.timeScale = 1;
                this.paused = false;
                this.cursorController.LockCursor();
                if (GameObject.Find("Settings").activeSelf)
                {
                    (GameObject.Find("Settings")).SetActive(false);
                }
            }
        }
			this.UpdateItemName();
			if (this.ps.gameOver)
			{
				this.gameAudio.PlayOneShot(jumpscare, 1f);
                base.GetComponent<SoundType>().soundType = SoundType.soundThing.SFX;
                daveObject.GetComponent<NavMeshAgent>().speed = 0f;
			}
			if (this.jumpscareTime > 0f)
			{
				this.jumpscareTime -= 1f * Time.deltaTime;
			}
			if (this.jumpscareTime <= 0f & this.ps.gameOver)
			{
				SceneManager.LoadScene("Menu");
				this.cursorController.UnlockCursor();
			}
        if(gogglesAreBeingUsed)
        {
            goggleTime -= 1f * Time.deltaTime;
            if(goggleTime <= 0f)
            {
                gogglesAreBeingUsed = false;
            }
        }
        if(!gogglesAreBeingUsed)
        {
            goggleOverlay.SetActive(false);
        }
        daveDistance = Vector3.Distance(player.position, dave.position);
        distanceFromDave.text = "DISTANCE FROM DAVE: " + daveDistance.ToString();
        UseItem();
        timeSinceSceneBegan = Time.time;
        if(timeSinceSceneBegan >= 180)
        {
            if(mode == "timed")
            {
                if(chaseMode)
                {
                    ps.gameOver = true;
                }
            }
        }
     }
     public void UpdateSupplyCount()
     {
     	   this.supplyText.text = "SUPPLIES: " + this.supplies.ToString() + "/8";
	   if (this.supplies == 1)
	   {
	   	this.normalDaveAudio.PlayOneShot(prize, 1f);
            base.GetComponent<SoundType>().soundType = SoundType.soundThing.Voice;
            this.coin.SetActive(true);
	   }
	   if (this.supplies == 2)
	   {
            ChaseMode();
	   }
       if(this.supplies == 8 & mode == "normal")
        {
            RenderSettings.ambientLight = Color.black;
            this.supplyText.text = "ESCAPE";
        }
     }
     public void CollectSupplies()
     {
     	   this.supplies++;
	   this.UpdateSupplyCount();
     }
     public void UpdateItemName()
     {
     if (this.item == 0)
	 {
	 	this.itemText.text = "Nothing";
		this.itemSprite.GetComponent<Image>().sprite = this.nothing;
	 }
	 if (this.item == 1)
	 {
	 	this.itemText.text = "Rare Coin";
		this.itemSprite.GetComponent<Image>().sprite = this.rareCoin;
	 }
        if (this.item == 2)
        {
            this.itemText.text = "Dave's Goggles";
            this.itemSprite.GetComponent<Image>().sprite = this.goggles;
        }
        if (this.item == 3)
        {
            this.itemText.text = "Tape";
            this.itemSprite.GetComponent<Image>().sprite = this.tape;
        }
        if(this.item == 4)
        {
            this.itemText.text = "Pie";
            this.itemSprite.GetComponent<Image>().sprite = this.pie;
        }
        if(this.item == 5)
        {
            this.itemText.text = "Slice of Cake";
            this.itemSprite.GetComponent<Image>().sprite = this.cake;
        }
        if (this.item == 6)
        {
            this.itemText.text = "Hammer";
            this.itemSprite.GetComponent<Image>().sprite = this.hammer;
        }
        if(item == 7)
        {
            this.itemSprite.GetComponent<Image>().sprite = this.teleporterItemSPR;
            itemText.text = "Teleporter";
        }
    }
    public void UseItem()
    {
        if(Input.GetKeyDown(input))
        {
            if (item == 2)
            {
                goggleTime = 5f;
                goggleOverlay.SetActive(true);
                gogglesAreBeingUsed = true;
                item = 0;
            }
            if(item == 4)
            {
                UnityEngine.Object.Instantiate<GameObject>(this.movingPie, this.player.position, this.cameraTransform.rotation);
                item = 0;
            }
            if(item == 5)
            {
                playerScript.staminaValue = 100f;
                item = 0;
            }
            if(item == 7)
            {
                item = 0;
                StartCoroutine(teleporterItem());
            }
        }
    }
    public void PostProcess(bool isOn)
    {
        if(isOn)
        {
            postFX.SetActive(true);
        }
        if(!isOn)
        {
            postFX.SetActive(false);
        }
    }
    public void ChaseMode()
    {
        chaseMode = true;
        this.schoolAudio.Stop();
        this.normalDave.SetActive(false);
        this.daveObject.SetActive(true);
        itemGiver.SetActive(true);
    }

    public IEnumerator teleporterItem()
    {
        TeleportingPlayer = true;
        player.position = teleportationPositions[Random.Range(0, teleportationPositions.Length)].position + Vector3.up * 6f;
        gameAudio.PlayOneShot(PlaceHolderTeleportSound);
        base.GetComponent<SoundType>().soundType = SoundType.soundThing.SFX;
        yield return new WaitForSeconds(0.3f);
        player.position = teleportationPositions[Random.Range(0, teleportationPositions.Length)].position + Vector3.up * 6f;
        gameAudio.PlayOneShot(PlaceHolderTeleportSound);
        base.GetComponent<SoundType>().soundType = SoundType.soundThing.SFX;
        yield return new WaitForSeconds(0.3f);
        player.position = teleportationPositions[Random.Range(0, teleportationPositions.Length)].position + Vector3.up * 6f;
        gameAudio.PlayOneShot(PlaceHolderTeleportSound);
        base.GetComponent<SoundType>().soundType = SoundType.soundThing.SFX;
        yield return new WaitForSeconds(0.3f);
        gameAudio.PlayOneShot(PlaceHolderTeleportSound);
        base.GetComponent<SoundType>().soundType = SoundType.soundThing.SFX;
        player.position = teleportationPositions[Random.Range(0, teleportationPositions.Length)].position + Vector3.up * 6f;
        yield return new WaitForSeconds(0.3f);
        gameAudio.PlayOneShot(PlaceHolderTeleportSound);
        base.GetComponent<SoundType>().soundType = SoundType.soundThing.SFX;
        player.position = teleportationPositions[Random.Range(0, teleportationPositions.Length)].position + Vector3.up * 6f;
        yield return new WaitForSeconds(0.3f);
        TeleportingPlayer = false;
        yield break;
    }
}
