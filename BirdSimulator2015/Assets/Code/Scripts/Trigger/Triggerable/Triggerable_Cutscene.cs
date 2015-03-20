﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_Cutscene : BaseTriggerable<BaseTriggerable>
{
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
        if (!uLink.Network.isServer)
            return;

        if (!players.Contains(c.gameObject))
        {
            players.Add(c.gameObject);
        }
        else
        {
            players.Remove(c.gameObject);
        }

        if (players.Count >= Registry.Constant.PLAYERS && locked)
        {
            Debug.Log("Cutscene triggered");
            locked = false;

            Debug.Log("Linking players");
            if (players.Count > 1)
            {
                GameObject tether = uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.Tether, Registry.Prefab.Tether, Registry.Prefab.Tether, this.transform.position, Quaternion.identity, 0);
                tether.GetComponent<Tether>().attached1 = players[0];
                tether.GetComponent<Tether>().attached2 = players[1];
            }

            StartCoroutine(coStartCutscene());
            this.networkView.RPC("RPC_StartCutscene", uLink.RPCMode.Others);

            base.Trigger(c, g);
        }
    }

    IEnumerator coStartCutscene()
    {
        GameController.SetInputLock(true);
        GameController.CinematicPopup.FadeIn();

        yield return new WaitForSeconds(5f);
        GameController.Player.GetComponent<PlayerInput>().SetBool("b_Grounded", false);

        yield return new WaitForSeconds(2f);
        GameController.Player.GetComponent<PlayerInput>().SetTrigger("t_DashForward");

        yield return new WaitForSeconds(1f);
        GameController.CinematicPopup.FadeOut();

        GameController.Player.GetComponent<PlayerState>().SetSpeedyMode(true, Vector3.right);
        GameController.SetInputLock(false);

        yield return null;
    }

    [RPC]
    public void RPC_StartCutscene()
    {
        StartCoroutine(coStartCutscene());
    }
}
