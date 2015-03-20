using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_GodRay : BaseTriggerable<BaseTriggerable>
{
    public GameObject Light;
    public float Intensity = 8;
    public float MaxAlpha = 40f;
    public float Speed = 0.1f;
    private List<GameObject> players = new List<GameObject>();

    void Start()
    {
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        if (!players.Contains(c.gameObject))
        {
            players.Add(c.gameObject);
        }
        else
        {
            players.Remove(c.gameObject);
        }

        if (players.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(coOpenGodRay());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(coCloseGodRay());
        }
    }

    IEnumerator coOpenGodRay()
    {
        while (true)
        {
            if (Light.GetComponent<Light>().intensity < Intensity)
            {
                Light.GetComponent<Light>().intensity += Time.deltaTime;
            }
            if (Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color.a < MaxAlpha / 255f)
            {
                Color c = Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
                c.a += Time.deltaTime * Speed;
                Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
            }

            yield return null;
        }
    }

    IEnumerator coCloseGodRay()
    {
        while (true)
        {
            if (Light.GetComponent<Light>().intensity > 0)
            {
                Light.GetComponent<Light>().intensity -= Time.deltaTime;
            }
            if (Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color.a > 0f)
            {
                Color c = Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
                c.a -= Time.deltaTime * 0.1f;
                Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
            }

            yield return null;
        }
    }
}
