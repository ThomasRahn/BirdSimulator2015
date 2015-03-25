using UnityEngine;
using System.Collections;

public class Triggerable_EvilPillar : BaseTriggerable<BaseTriggerable>
{
    private bool alreadyTriggered = false;


	void Start()
    {
	}
	
	void Update()
    {
	}

    public override void Trigger(Collider c, GameObject g)
    {
        if (alreadyTriggered)
            return;

        alreadyTriggered = true;

        GameController.SetInputLock(true);
        //GameObject.FindWithTag(Registry.Tag.AudioController).GetComponent<AudioController>().FadeOut();
        GameController.CinematicPopup.FadeIn();
        GameController.Player.GetComponent<Rigidbody>().velocity = GameController.Player.GetComponent<PlayerState>().SpeedyModeForward;

        StartCoroutine(coMoveTowards());
        StartCoroutine(coFadeSound());
        base.Trigger(c, g);
    }

    IEnumerator coFadeSound()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.FindWithTag(Registry.Tag.AudioController).GetComponent<AudioController>().FadeOut();
    }

    IEnumerator coMoveTowards()
    {
        Vector3 dest;
        dest.x = GameController.Player.transform.position.x;
        dest.y = -888f;

        if (uLink.Network.isServer)
            dest.z = 15f;
        else
            dest.z = -15f;

        while (Vector3.Distance(GameController.Player.transform.position, dest) > 1f)
        {
            dest.x = GameController.Player.transform.position.x + 50f;

            GameController.Player.GetComponent<Rigidbody>().MovePosition(GameController.Player.transform.position + (dest - GameController.Player.transform.position) * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }
}
