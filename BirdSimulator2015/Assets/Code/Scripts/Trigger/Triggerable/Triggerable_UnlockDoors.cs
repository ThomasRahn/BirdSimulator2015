﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_UnlockDoors : BaseTriggerable<BaseTriggerable>
{
    public enum UnlockType
    {
        Slide,
        Pivot,

    }

    public float Length;
    public bool Lock; // lock instead of unlock
    public UnlockType Type;
    public Transform Left;
    public Transform Right;

    private List<GameObject> players = new List<GameObject>();
    private bool locked = true;

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

        if (players.Count >= Registry.Constant.PLAYERS && locked)
        {
            locked = false;
            StartCoroutine(coUnlockDoors());
            networkView.RPC("RPC_UnlockDoors", uLink.RPCMode.Others);

            GameController.Player.GetComponent<PlayerRumble>().BumbleRumble(7.5f, 0.2f, 0.2f);
            Left.GetComponentInChildren<AudioSource>().Play();
            Right.GetComponentInChildren<AudioSource>().Play();

            base.Trigger(c, g);
        }
	}

    IEnumerator coUnlockDoors()
    {
        yield return new WaitForSeconds(2f);

        int l = 1;
        if (Lock) l = -1;

        while (Length > 0)
        {
            if (Type == UnlockType.Pivot)
            {
                Left.transform.Rotate(Vector3.up, 1f);
                Right.transform.Rotate(Vector3.up, -1f);
            }
            else if (Type == UnlockType.Slide)
            {
                Left.transform.Translate(l * Vector3.right * Time.deltaTime);
                Right.transform.Translate(l * Vector3.left * Time.deltaTime);
            }

            Length -= Time.deltaTime;
            yield return null;
        }

        Left.GetComponentInChildren<AudioSource>().Stop();
        Right.GetComponentInChildren<AudioSource>().Stop();

        yield return null;
    }

    [RPC]
    public void RPC_UnlockDoors()
    {
        StartCoroutine(coUnlockDoors());
    }
}
