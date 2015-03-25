using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Egg : MonoBehaviour
{
	private List<GameObject> links;
	private Vector3 spawn;
	private PlayerState player;

	private void Start()
    {
		links = new List<GameObject>();
		spawn = transform.parent.position;
	}
	
    private void OnTriggerEnter(Collider c)
    {
        if (c.tag == Registry.Tag.Player)
        {
			if(uLink.NetworkView.Get(this).isOwner)
			{
				player = c.GetComponent<PlayerState>();
				player.HeldEgg = this;

				// Attach owner to egg
				link(c.attachedRigidbody);

				// Disable the pickup on networked eggs
				uLink.NetworkView.Get(this).RPC("ToggleCollider", uLink.RPCMode.Others, false);
			}
			else // Give ownership to the player that picked it up
			{
				uLink.Network.Destroy(transform.parent.gameObject);

				if(uLink.Network.isServer)
				{
					uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, 
				                          	  Registry.Prefab.Egg, transform.parent.position, transform.parent.rotation, 0);
				}
				else
				{
					uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, 
					                          Registry.Prefab.EggProxy, transform.parent.position, transform.parent.rotation, 0);
				}
			}
        }
    }

	[RPC]
	public void ToggleCollider(bool enabled)
	{
		GetComponent<Collider>().enabled = enabled;
	}

	private void link(Rigidbody bird)
	{
		Rigidbody egg = transform.parent.GetComponent<Rigidbody>();
		egg.useGravity = true;

		Vector3 initialPosition = transform.parent.position;
		initialPosition.y = transform.parent.GetComponent<Renderer>().bounds.max.y;

		links = ChainLinker.NetworkLink(initialPosition, bird.transform.position, egg.GetComponent<Joint>(), bird, true);
		GetComponent<Collider>().enabled = false; // Prevent picking up the egg again
	}

	public void Detach()
	{
		for(int i = 0; i < links.Count; i++)
		{
			uLink.Network.Destroy(links[i]);
		}
		links.Clear();

		Rigidbody eggBody = GetComponentInParent<Rigidbody>();
		eggBody.useGravity = false;
		eggBody.angularVelocity = Vector3.zero;
		eggBody.velocity = Vector3.zero;
		eggBody.transform.rotation = Quaternion.identity;

		if(player != null)
		{
			player.HeldEgg = null;
		}
	}

	public void Reset()
	{
		Detach();
		uLink.NetworkView.Get(this).RPC("ToggleCollider", uLink.RPCMode.All, true);
		transform.parent.position = spawn;
	}
}
