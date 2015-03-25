using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingPopup : MonoBehaviour
{
    private const float FADE_IN_TIME = 3f;

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

    public void FadeIn()
    {
        StopAllCoroutines();
        image.CrossFadeAlpha(1f, FADE_IN_TIME, false);
    }
}
