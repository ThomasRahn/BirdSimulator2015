using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CinematicPopup : MonoBehaviour
{
    public Image Top;
    public Image Bottom;

    private const float FADE_IN_TIME = 3f;
    private const float FADE_OUT_TIME = 3f;
    private Image top;
    private Image bottom;

    void Awake()
    {
        top = Top;
        bottom = Bottom;
        top.CrossFadeAlpha(0f, 0f, false);
        bottom.CrossFadeAlpha(0f, 0f, false);
    }

	void Start()
    {
	}
	
	void Update()
    {
	}

    public void FadeIn()
    {
        top.CrossFadeAlpha(1f, FADE_IN_TIME, false);
        bottom.CrossFadeAlpha(1f, FADE_IN_TIME, false);
    }

    public void FadeOut()
    {
        top.CrossFadeAlpha(0f, FADE_OUT_TIME, false);
        bottom.CrossFadeAlpha(0f, FADE_OUT_TIME, false);
    }
}
