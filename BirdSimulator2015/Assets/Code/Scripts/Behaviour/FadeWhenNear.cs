using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeWhenNear : MonoBehaviour
{
    private const float FADE_OUT_TIME = 2f;
    private const float DISTANCE = 20f;
    private bool triggered = false;

	void Start()
    {
        this.GetComponent<Image>().CrossFadeAlpha(1f, 0f, false);
	}

	void Update()
    {
        if (GameController.Player == null)
            return;

        Debug.Log(Vector3.Distance(this.transform.position, GameController.Player.transform.position));

        if (!triggered && Vector3.Distance(this.transform.position, GameController.Player.transform.position) < DISTANCE)
        {
            triggered = true;
            fadeOut();
        }
	}

    void fadeOut()
    {
        StartCoroutine(coFadeOut());
    }

    IEnumerator coFadeOut()
    {
        this.GetComponent<Image>().CrossFadeAlpha(0f, FADE_OUT_TIME, false);
        yield return null;
    }
}
