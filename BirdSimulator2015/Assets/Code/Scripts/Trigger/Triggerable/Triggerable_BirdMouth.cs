using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_BirdMouth : BaseTriggerable<BaseTriggerable>
{
    public float Speed;

    private Vector3 original;
    private List<GameObject> players = new List<GameObject>();

    void Awake()
    {
        original = this.transform.localPosition;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        if (!players.Contains(g))
        {
            players.Add(g);
        }
        else
        {
            players.Remove(g);
        }

        if (players.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(coOpenSesame());
            networkView.RPC("RPC_BirdMouth_Open", uLink.RPCMode.Others);
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(coCloseSesame());
            networkView.RPC("RPC_BirdMouth_Close", uLink.RPCMode.Others);
        }
        base.Trigger(c, g);
    }

    IEnumerator coOpenSesame()
    {
        while (Mathf.Abs(this.transform.localPosition.y - original.y) < 60)
        {
            this.transform.Translate(Vector3.down * Time.deltaTime * Speed);
            yield return null;
        }

        yield return null;
    }

    IEnumerator coCloseSesame()
    {
        Debug.Log(this.transform.localPosition.y - original.y);
        while (this.transform.localPosition.y - original.y < 0)
        {
            this.transform.Translate(Vector3.up * Time.deltaTime * Speed);
            yield return null;
        }

        yield return null;
    }

    [RPC]
    public void RPC_BirdMouth_Open()
    {
        StartCoroutine(coOpenSesame());
    }

    [RPC]
    public void RPC_BirdMouth_Close()
    {
        StartCoroutine(coCloseSesame());
    }
}
