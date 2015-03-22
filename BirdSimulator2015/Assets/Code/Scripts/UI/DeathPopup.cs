using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathPopup : MonoBehaviour
{
    private const float FADE_IN_TIME = 3f;
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

    public void Activate()
    {
        FadeIn();
        FadeOut();
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
