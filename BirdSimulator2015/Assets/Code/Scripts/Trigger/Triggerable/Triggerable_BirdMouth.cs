using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_BirdMouth : BaseTriggerable<BaseTriggerable>
{
    public float Speed;
    public Transform BirdMouth;

    private Vector3 original;
    private List<GameObject> players = new List<GameObject>();

    void Awake()
    {
        original = BirdMouth.transform.localPosition;
    }

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
            StartCoroutine(coOpenSesame());
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
        while (Mathf.Abs(BirdMouth.transform.localPosition.y - original.y) < 50)
        {
            BirdMouth.Translate(Vector3.down * Time.deltaTime * Speed);
            yield return null;
        }

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
