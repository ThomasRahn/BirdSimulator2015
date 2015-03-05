using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GamepadPopup : MonoBehaviour
{
    private const float FADE_IN_TIME = 2f;
    private const float FADE_OUT_TIME = 2f;
    private const float FADE_OUT_DELAY = 4f;

    private Image image;

    void Awake()
    {
        image = this.GetComponent<Image>();
        // since unity kindly uses the canvasrenderer instead of the images own alpha
        // we need to use this fucking workaround
        image.CrossFadeAlpha(0f, 0f, false);
    }

	void Start()
    {
	}
	
	void Update()
    {
	}

    public void SetImage(GamepadSetup.GamepadAction ia)
    {
        image.sprite = GameController.Gamepad.GetSprite(ia);
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        image.CrossFadeAlpha(1f, FADE_IN_TIME, false);
    }

    public void FadeOut()
    {
        StartCoroutine(coFadeOut());
    }

    IEnumerator coFadeOut()
    {
        yield return new WaitForSeconds(FADE_OUT_DELAY);
        image.CrossFadeAlpha(0f, FADE_OUT_TIME, false);
        yield return null;
    }
}
