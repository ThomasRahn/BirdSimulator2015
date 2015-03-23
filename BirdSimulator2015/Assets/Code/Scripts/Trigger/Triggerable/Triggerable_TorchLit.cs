using UnityEngine;
using System.Collections;

public class Triggerable_TorchLit : BaseTriggerable<BaseTriggerable> 
{
	public override void Trigger(Collider c, GameObject g)
	{
        if (!uLink.Network.isServer)
            return;

		Flame flame = this.GetComponentInChildren<Flame>();
		if (!flame.IsLit)
        {
			flame.ToggleLit();
            GameObject.Find(Registry.Prefab.CloudWall).GetComponent<CloudWall>().PushBack();
			networkView.RPC("LightTorch", uLink.RPCMode.Others);
		}

		base.Trigger(c, g);
	}

	[RPC]
    public void LightTorch()
	{
		Flame flame = this.GetComponentInChildren<Flame>();
		if (!flame.IsLit)
        {
			flame.ToggleLit();
			GameObject.Find("CloudWall").GetComponent<CloudWall>().PushBack();
		}
	}
}
