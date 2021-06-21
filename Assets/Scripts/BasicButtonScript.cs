using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Token: 0x02000005 RID: 5
public class BasicButtonScript : MonoBehaviour
{
	// Token: 0x0600000C RID: 12 RVA: 0x00002216 File Offset: 0x00000616
	private void Start()
	{
		this.button = base.GetComponent<Button>();
		this.button.onClick.AddListener(new UnityAction(this.OpenScreen));
	}
    void Update()
    {
        hour = DateTime.Now.Hour;
        minute = DateTime.Now.Minute;
    }
	// Token: 0x0600000D RID: 13 RVA: 0x00002240 File Offset: 0x00000640
	private void OpenScreen()
	{
		this.screen.SetActive(true);
        this.screenToRemove.SetActive(false);
	}
    
	// Token: 0x0400000C RID: 12
	private Button button;

	// Token: 0x0400000D RID: 13
	public GameObject screen;

    public GameObject screenToRemove;

    public int hour;

    public int minute;
}
