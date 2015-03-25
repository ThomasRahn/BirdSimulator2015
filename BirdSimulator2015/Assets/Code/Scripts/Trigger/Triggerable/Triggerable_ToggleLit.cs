using UnityEngine;
using System.Collections;

public class Triggerable_ToggleLit : BaseTriggerable<BaseTriggerable>
{
    void Start()
    {
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        this.GetComponentInChildren<Flame>().ToggleLit();
        base.Trigger(c, g);
    }
}
