using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationPopup : MonoBehaviour
{
    private const float FADE_IN_TIME = 1f;
    private const float FADE_OUT_TIME = 1f;
    private const float FADE_OUT_DELAY = 4f;

	private Text text;

	void Awake()
	{
		text = this.GetComponent<Text>();
		text.CrossFadeAlpha(0f, 0f, false);
	}
	
	void Start()
	{
	}

	void Update()
	{
	}

	public void SetText(string s)
	{
		text.text = s;
	}

	public void Popup()
	{
		fadeIn();
		fadeOut();
	}

	void fadeIn()
	{
		StopAllCoroutines();
		text.CrossFadeAlpha(1f, FADE_IN_TIME, false);
	}
	
	void fadeOut()
	{
		StartCoroutine(coFadeOut());
	}
	
	IEnumerator coFadeOut()
	{
		yield return new WaitForSeconds(FADE_OUT_DELAY);
		text.CrossFadeAlpha(0f, FADE_OUT_TIME, false);
		yield return null;
	}
}
