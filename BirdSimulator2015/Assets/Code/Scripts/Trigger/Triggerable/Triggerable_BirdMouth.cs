using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_BirdMouth : BaseTriggerable<BaseTriggerable>
{
    public float Speed;
    public Transform BirdMouth;
    public GameObject EyeLeft;
    public GameObject EyeRight;
    public ParticleSystem DustLeft;
    public ParticleSystem DustRight;

    private Vector3 original;
    private List<GameObject> players = new List<GameObject>();

    void Awake()
    {
        original = BirdMouth.transform.localPosition;
    }

    void Start()
    {
        EyeLeft.GetComponent<MeshRenderer>().sharedMaterial.EnableKeyword("_EMISSION");
        EyeLeft.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_EmissionColor", new Color(0.1f, 0.1f, 0.1f, 0f));
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
            DustLeft.Play();
            DustRight.Play();
            StopAllCoroutines();
            StartCoroutine(coOpenSesame());

            this.GetComponent<AudioSource>().Play();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(coCloseSesame());
        }
        base.Trigger(c, g);
    }

    IEnumerator coOpenSesame()
    {
        while (Mathf.Abs(BirdMouth.transform.localPosition.y - original.y) < 45)
        {
            if (EyeLeft.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_EmissionColor").r < 1f)
            {
                Color c = EyeLeft.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_EmissionColor");
                c.r += Time.deltaTime * 0.1f;
                c.g += Time.deltaTime * 0.1f;
                c.b += Time.deltaTime * 0.1f;
                EyeLeft.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_EmissionColor", c);
            }

            BirdMouth.Translate(Vector3.down * Time.deltaTime * Speed);
            yield return null;
        }

        DustLeft.Stop();
        DustRight.Stop();

        yield return null;
    }

    IEnumerator coCloseSesame()
    {
        //Debug.Log(this.transform.localPosition.y - original.y);
        while (BirdMouth.transform.localPosition.y - original.y < 0)
        {
            BirdMouth.Translate(Vector3.up * Time.deltaTime * Speed);
            yield return null;
        }

        yield return null;
    }
}
