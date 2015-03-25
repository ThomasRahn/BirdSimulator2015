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

				// Attach proxies to egg on other machines
				uLink.NetworkView.Get(this).RPC("Attach", uLink.RPCMode.Others, c.transform.position);
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
	public void Attach(Vector3 birdPosition)
	{
		GetComponent<Collider>().enabled = false;
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
		GetComponent<Collider>().enabled = true;
		transform.parent.position = spawn;
	}
}
