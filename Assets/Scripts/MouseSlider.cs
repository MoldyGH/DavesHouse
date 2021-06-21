using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000018 RID: 24
public class MouseSlider : MonoBehaviour
{
	// Token: 0x06000078 RID: 120 RVA: 0x000050DF File Offset: 0x000034DF
	private void Start()
	{
		this.slider = base.GetComponent<Slider>();
		this.slider.value = PlayerPrefs.GetFloat("MouseSensitivity");
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00005102 File Offset: 0x00003502
	private void Update()
	{
		PlayerPrefs.SetFloat("MouseSensitivity", this.slider.value);
	}

	// Token: 0x040000D3 RID: 211
	public Slider slider;
}
