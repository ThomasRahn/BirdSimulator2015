using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeWhenNear : MonoBehaviour
{
    public bool FadeIn = false; // fade in or out
    public float Distance = 0f;
    public float Time = 2f;

    private bool triggered = false;

	void Start()
    {
        if (FadeIn)
        {
            this.GetComponent<Image>().CrossFadeAlpha(0f, 0f, false);
        }
        else
        {
            this.GetComponent<Image>().CrossFadeAlpha(1f, 0f, false);
        }

	}

	void Update()
    {
        if (GameController.Player == null)
            return;

        if (!triggered && Vector3.Distance(this.transform.position, GameController.Player.transform.position) < Distance)
        {
            triggered = true;
            if (FadeIn)
            {
                fadeIn();
            }
            else
            {
                fadeOut();
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (GameController.Player == null)
            return;

        if (!triggered)
        {
            triggered = true;
            if (FadeIn)
            {
                fadeIn();
            }
            else
            {
                fadeOut();
            }
        }
    }

    void fadeIn()
    {
        StartCoroutine(coFadeIn());
    }

    IEnumerator coFadeIn()
    {
        this.GetComponent<Image>().CrossFadeAlpha(1f, Time, false);
        yield return null;
    }

    void fadeOut()
    {
        StartCoroutine(coFadeOut());
    }

    IEnumerator coFadeOut()
    {
        this.GetComponent<Image>().CrossFadeAlpha(0f, Time, false);
        yield return null;
    }
}
