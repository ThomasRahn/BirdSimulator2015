using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_PillarKaboom : BaseTriggerable<BaseTriggerable>
{
    public GameObject[] Pillars;
    public GameObject Kabooms;

    private bool triggered = false;

    void Start()
    {
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        if (triggered)
            return;

        triggered = true;

        Camera.main.GetComponent<BirdSimulator2015.Code.Scripts.Cam.TPRadialCamera>().TargetRadius = 200f;

        int children = Kabooms.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Kabooms.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
            Kabooms.transform.GetChild(i).GetChild(0).GetComponent<ParticleSystem>().Play();
        }

        this.GetComponent<AudioSource>().Play();

        StartCoroutine(coMoveShit());
        StartCoroutine(coSaturate());
        StartCoroutine(coFadeOut());

        base.Trigger(c, g);
    }

    IEnumerator coMoveShit()
    {
        float t = 10f;

        Vector3[] directions = new Vector3[Pillars.Length];

        for (int i = 0; i < directions.Length; i++)
        {
            float forward = Random.Range(-1f, 1f);
            float right = Random.Range(-1f, 1f);
            float up = Random.Range(-1f, 1f);
            directions[i] = (Vector3.forward * forward) + (Vector3.right * right) + (Vector3.up * up);
        }


        while (t > 0)
        {
            t -= Time.deltaTime;

            int i = 0;
            foreach (GameObject g in Pillars)
            {
                g.transform.Translate(directions[i] * Time.deltaTime * 20f);
                i++;
            }
            if (t < 9)
            {
                Time.timeScale = 0.3f;
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator coSaturate()
    {
        yield return new WaitForSeconds(3f);
        while (Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation < 1.5f)
        {
            Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    IEnumerator coFadeOut()
    {
        yield return new WaitForSeconds(5f);
        GameController.EndingPopup.FadeIn();
        yield return null;
    }
}
