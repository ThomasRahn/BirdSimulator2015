using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LocatorPopup : MonoBehaviour
{
    public Image Onscreen;
    public Image Offscreen;

    private const float FADE_IN_TIME = 4f;
    private const float FADE_OUT_TIME = 4f;

    private float fadeTimer = -1f;

    void Start()
    {
        Onscreen.CrossFadeAlpha(0f, 0f, false);
        Offscreen.CrossFadeAlpha(0f, 0f, false);
    }

    void Update()
    {
        Onscreen.enabled = false;
        Offscreen.enabled = false;

        if (Camera.main == null || GameObject.FindWithTag("Proxy") == null)
            return;

        fadeTimer -= Time.deltaTime;
        if (fadeTimer < 0)
        {
            FadeOut();
            return;
        }
        FadeIn();

        Vector3 screenpos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Proxy").transform.position);

        if (screenpos.z > 0 && screenpos.x < Screen.width && screenpos.x > 0 && screenpos.y < Screen.height && screenpos.y > 0)
        {
            screenpos += Vector3.left * 32f + Vector3.down * 32f;
            Onscreen.enabled = true;
            Onscreen.rectTransform.position = screenpos;
        }
        else
        {
            float x = screenpos.x;
            float y = screenpos.y;
            float offset = 10;

            if (screenpos.z < 0)
            {
                screenpos = -screenpos;
            }

            if (screenpos.x > Screen.width)
            {
                x = Screen.width - offset;
            }
            if (screenpos.x < 0)
            {
                x = offset;
            }

            if (screenpos.y > Screen.height)
            {
                y = Screen.height - offset;
            }
            if (screenpos.y < 0)
            {
                y = offset;
            }

            x -= 32f;
            y -= 32f;
            Offscreen.enabled = true;
            Offscreen.rectTransform.position = new Vector3(x, y, 0);
        }
    }

    public void ResetFade()
    {
        fadeTimer = 3f;
    }

    public void FadeIn()
    {
        Onscreen.CrossFadeAlpha(1f, FADE_IN_TIME, false);
        Offscreen.CrossFadeAlpha(1f, FADE_IN_TIME, false);
    }

    public void FadeOut()
    {
        Onscreen.CrossFadeAlpha(0f, FADE_OUT_TIME, false);
        Offscreen.CrossFadeAlpha(0f, FADE_OUT_TIME, false);
    }
}
