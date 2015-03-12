using UnityEngine;
using System.Collections;

public class Tauntable_Torch : Tauntable
{
    public override void Taunted(GameObject g)
    {
		this.GetComponentInChildren<Flame>().ToggleLit();
    }
}
