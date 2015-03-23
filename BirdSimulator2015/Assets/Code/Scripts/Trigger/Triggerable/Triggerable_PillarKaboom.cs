using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_PillarKaboom : BaseTriggerable<BaseTriggerable>
{
    public GameObject PillarA;
    public GameObject PillarB;
    public GameObject PillarC;
    public GameObject PillarD;

    public GameObject Kabooms;

    void Start()
    {
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        Camera.main.GetComponent<BirdSimulator2015.Code.Scripts.Cam.TPRadialCamera>().TargetRadius = 200f;
        Time.timeScale = 0.5f;

        StartCoroutine(coMoveShit());

        int children = Kabooms.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Kabooms.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
            Kabooms.transform.GetChild(i).GetChild(0).GetComponent<ParticleSystem>().Play();
        }

        StartCoroutine(coFadeOut());

        base.Trigger(c, g);
    }

    IEnumerator coMoveShit()
    {
        float t = 10f;

        while (t > 0)
        {
            t -= Time.deltaTime;
            PillarA.transform.Translate((Vector3.down + Vector3.forward) * Time.deltaTime * 2f);
            PillarB.transform.Translate((Vector3.down + Vector3.back) * Time.deltaTime * 2f);
            PillarC.transform.Translate((Vector3.down + Vector3.forward) * Time.deltaTime * 2f);
            PillarD.transform.Translate((Vector3.down + Vector3.back) * Time.deltaTime * 2f);
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
