using UnityEngine;
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

        // TODO REPLACE 0 WITH 1 FOR RELEASE
        if (players.Count > 0 && locked)
        {
            locked = false;
            StartCoroutine(coUnlockDoors());
            base.Trigger(c, g);
        }
	}

    IEnumerator coUnlockDoors()
    {
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

        yield return null;
    }
}
