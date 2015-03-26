using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationPopup : MonoBehaviour
{
    public enum Location
    {
        RAVENHOME,
        ATRIUM,
        INNERSANCTUM,
        ESCAPE,

    }

    public Sprite Ravenhome;
    public Sprite Atrium;
    public Sprite InnerSanctum;
    public Sprite Escape;

    private const float FADE_IN_TIME = 1f;
    private const float FADE_OUT_TIME = 1f;
    private const float FADE_OUT_DELAY = 4f;

	private Image image;

	void Awake()
	{
		image = this.GetComponent<Image>();
		image.CrossFadeAlpha(0f, 0f, false);
	}
	
	void Start()
	{
	}

	void Update()
	{
	}

	public void SetText(Location l)
	{
        if (l == Location.RAVENHOME)
		    image.sprite = Ravenhome;
        if (l == Location.ATRIUM)
            image.sprite = Atrium;
        if (l == Location.INNERSANCTUM)
            image.sprite = InnerSanctum;
        if (l == Location.ESCAPE)
            image.sprite = Escape;
	}

	public void Popup()
	{
		fadeIn();
		fadeOut();
	}

	void fadeIn()
	{
		StopAllCoroutines();
		image.CrossFadeAlpha(1f, FADE_IN_TIME, false);
	}
	
	void fadeOut()
	{
		StartCoroutine(coFadeOut());
	}
	
	IEnumerator coFadeOut()
	{
        float offset = 0f;
        if (image.sprite == Ravenhome)
        {
            offset = 4f;
        }

        yield return new WaitForSeconds(FADE_OUT_DELAY + offset);
		image.CrossFadeAlpha(0f, FADE_OUT_TIME, false);
		yield return null;
	}
}
