using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationPopup : MonoBehaviour
{
    private const float FADE_IN_TIME = 1f;
    private const float FADE_OUT_TIME = 1f;
    private const float FADE_OUT_DELAY = 4f;
    private bool dispose = false;

    void Awake()
    {
        this.GetComponent<Text>().CrossFadeAlpha(0f, 0f, false);
    }

	void Start()
    {
        this.GetComponent<Text>().CrossFadeAlpha(1f, FADE_IN_TIME, false);
        StartCoroutine(coFadeOut());
	}
	
	void Update()
    {
        if (dispose)
        {
            GameObject.Destroy(this.gameObject);
        }
	}

    IEnumerator coFadeOut()
    {
        yield return new WaitForSeconds(FADE_OUT_DELAY);
        this.GetComponent<Text>().CrossFadeAlpha(0f, FADE_OUT_TIME, false);
        yield return new WaitForSeconds(FADE_OUT_TIME);
        dispose = true;
        yield return null;
    }
}
