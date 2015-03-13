using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour
{
    public bool IsLit = true;

    private int currentFrame = 0;
    private int totalFrames;
    private float frameDelay = 0.03f;
    private float intensity = 1f;

    void Awake()
    {
        totalFrames = this.transform.childCount;
        for (int i = 0; i < totalFrames; i++)
        {
            this.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Start()
    {
        IsLit = !IsLit;
        ToggleLit();
    }

    void Update()
    {
        intensity = Random.Range(0.7f, 2f);
        this.light.intensity = Mathf.Lerp(this.light.intensity, intensity, 0.1f);
    }

    IEnumerator Flamey()
    {
        while (gameObject.activeSelf)
        {
            this.transform.GetChild(currentFrame).GetComponent<MeshRenderer>().enabled = false;
            currentFrame++;

            if (currentFrame == totalFrames)
            {
                currentFrame = 0;
            }

            this.transform.GetChild(currentFrame).GetComponent<MeshRenderer>().enabled = true;
            yield return new WaitForSeconds(frameDelay);
        }
    }

    public void ToggleLit()
    {
        IsLit = !IsLit;

        if (!IsLit)
        {
            StopAllCoroutines();
            this.transform.GetChild(currentFrame).GetComponent<MeshRenderer>().enabled = false;
            this.GetComponent<Light>().enabled = false;
        }
        else
        {
            StartCoroutine(Flamey());
            this.GetComponent<Light>().enabled = true;
        }
    }
}
