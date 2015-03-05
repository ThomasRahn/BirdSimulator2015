using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour
{
    private int currentFrame = 0;
    private int totalFrames;
    private float frameDelay = 0.03f;

    void Start()
    {
        totalFrames = this.transform.childCount;
        StartCoroutine(Flamey());
    }

    void Update()
    {
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
}
