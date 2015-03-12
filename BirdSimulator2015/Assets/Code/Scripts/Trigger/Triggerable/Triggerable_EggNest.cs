using UnityEngine;
using System.Collections;

public class Triggerable_EggNest : BaseTriggerable<BaseTriggerable>
{
    public GameObject[] Torches;
    public GameObject[] Pillars;

    private int eggz = 0;

    void Start()
    {
        Pillars[0].GetComponentInChildren<LandingZone>().enabled = false;
        Pillars[1].GetComponentInChildren<LandingZone>().enabled = false;
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        eggz++;

        c.transform.SetParent(null);
        c.transform.position = this.transform.position + Vector3.up;

        if (eggz == 1)
        {
            Torches[0].GetComponentInChildren<BaseTriggerable>().Trigger(c, this.gameObject);
        }
        else if (eggz == 2)
        {
            Torches[1].GetComponentInChildren<BaseTriggerable>().Trigger(c, this.gameObject);

            Pillars[0].GetComponentInChildren<LandingZone>().enabled = true;
            Pillars[1].GetComponentInChildren<LandingZone>().enabled = true;
        }

        base.Trigger(c, g);
    }
}
