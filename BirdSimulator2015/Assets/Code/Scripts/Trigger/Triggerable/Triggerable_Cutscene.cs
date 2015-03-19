using UnityEngine;
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
            Debug.Log("add" + c.name);
            players.Add(c.gameObject);
        }
        else
        {
            Debug.Log("remove" + c.name);
            players.Remove(c.gameObject);
        }

        Debug.Log("count" + players.Count);
        if (players.Count == Registry.Constant.PLAYERS && locked)
        {
            Debug.Log("go");
            locked = false;

            //GameObject tether = uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.Tether, Registry.Prefab.Tether, Registry.Prefab.Tether, this.transform.position, Quaternion.identity, 0);
            //tether.GetComponent<Tether>().attached1 = players[0];
            //tether.GetComponent<Tether>().attached2 = players[1];

            StartCoroutine(coStartCutscene());
            this.networkView.RPC("RPC_StartCutscene", uLink.RPCMode.Others);

            base.Trigger(c, g);
        }
    }

    IEnumerator coStartCutscene()
    {
        GameController.SetInputLock(true);
        GameController.CinematicPopup.FadeIn();
        Debug.Log(GameController.Player.GetComponent<PlayerState>().GetState());
        while (GameController.Player.GetComponent<PlayerState>().GetState() == PlayerState.BirdState.Grounded)
        {
            Debug.Log("asdad");
            GameController.Player.GetComponent<PlayerInput>().SetBool("b_Grounded", false);

        }
 

        yield return new WaitForSeconds(2f);
        GameController.Player.GetComponent<PlayerInput>().SetTrigger("t_DashForward");



        yield return null;
    }

    [RPC]
    public void RPC_StartCutscene()
    {
        StartCoroutine(coStartCutscene());
    }
}
