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

            if (players.Count > 1)
            {
				Debug.Log("Linking players");
                uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.Tether, Registry.Prefab.Tether, Registry.Prefab.Tether, this.transform.position, Quaternion.identity, 0);
            }

            StartCoroutine(coStartCutscene());
            this.networkView.RPC("RPC_StartCutscene", uLink.RPCMode.Others);

            base.Trigger(c, g);
        }
    }

    IEnumerator coStartCutscene()
    {
		CameraContainer camContainer = Camera.main.GetComponentInParent<CameraContainer>();
		camContainer.Radial(true); // Force the radial camera
		camContainer.LockCameraSystem(true); // Prevent the free cam from activating during respawn

        GameController.SetInputLock(true);
        GameController.CinematicPopup.FadeIn();
        GameObject.FindWithTag(Registry.Tag.AudioController).GetComponent<AudioController>().FadeOut();
        Camera.main.GetComponent<BirdSimulator2015.Code.Scripts.Cam.TPRadialCamera>().TargetRadius = 30f;

        yield return new WaitForSeconds(10f);
        GameController.Player.GetComponent<PlayerInput>().SetBool(Registry.Animator.Grounded, false);

        yield return new WaitForSeconds(2f);
        GameController.Player.GetComponent<PlayerInput>().SetTrigger(Registry.Animator.DashForward);

        Camera.main.GetComponent<BirdSimulator2015.Code.Scripts.Cam.TPRadialCamera>().TargetRadius = 20f;
        yield return new WaitForSeconds(0.1f);
        GameController.CinematicPopup.FadeOut();

        GameController.Player.GetComponent<PlayerState>().SetSpeedyMode(true, Vector3.right);

		GameObject.FindWithTag(Registry.Tag.AudioController).GetComponent<AudioController>().FadeIn(AudioController.BGMTrack.Chase);
        yield return new WaitForSeconds(3f);
        GameController.SetInputLock(false);

        yield return null;
    }

    [RPC]
    public void RPC_StartCutscene()
    {
        StartCoroutine(coStartCutscene());
    }
}
