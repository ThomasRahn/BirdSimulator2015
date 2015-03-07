using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_UnlockDoors : BaseTriggerable<BaseTriggerable>
{
    private List<GameObject> players = new List<GameObject>();
    private bool Locked = true;

    public Transform Left;
    public Transform Right;

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

        // TODO REPLACE 0 WITH 1 FOR RELEASE
        if (players.Count > 0 && Locked)
        {
            Locked = false;
            StartCoroutine(coUnlockDoors());
            base.Trigger(c, g);
        }
	}

    IEnumerator coUnlockDoors()
    {
        float t = 5.0f;

        Debug.Log("Unlock");
        while (t > 0)
        {
            Left.transform.Rotate(Vector3.up, 1f);
            Right.transform.Rotate(Vector3.up, -1f);

            t -= Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}
