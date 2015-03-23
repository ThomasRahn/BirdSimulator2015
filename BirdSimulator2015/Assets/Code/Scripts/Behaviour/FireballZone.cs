using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireballZone : uLink.MonoBehaviour
{
	public float Speed = 3f;
	public Transform Target;
    public bool IsMini = false;

	private List<Transform> targets = new List<Transform>();
	private Transform fireball;

	void Awake()
	{
        GameObject g;
        if (IsMini)
        {
            g = uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.FireballMini, Registry.Prefab.FireballMini, Registry.Prefab.FireballMini, this.transform.position, Quaternion.identity, 0);
        }
        else
        {
            g = uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.Fireball, Registry.Prefab.Fireball, Registry.Prefab.Fireball, this.transform.position + Vector3.up * 15f, Quaternion.identity, 0);
        }
		
		fireball = g.transform;
	}

	void Start()
	{
	}
	
	void Update()
	{
		if (targets.Count > 0)
		{
			Target = targets[0];
		}
		else
		{
			Target = this.transform;
		}

		fireball.transform.position = Vector3.MoveTowards(fireball.transform.position, Target.position, Time.deltaTime * Speed);
	}

	public void PushBack(Vector3 force){
		if (uLink.Network.isServer)
			fireball.GetComponent<Rigidbody> ().AddForce (force);
		else 
			fireball.GetComponent<uLinkNetworkView>().RPC("NetworkPushBack", uLink.RPCMode.Server, force);
	}

	[RPC]
	public void NetworkPushBack(Vector3 force)
	{
		fireball.GetComponent<Rigidbody> ().AddForce (force);
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.tag == Registry.Tag.Player || c.tag == Registry.Tag.Proxy)
		{
			targets.Add(c.transform);
		}
	}
	
	void OnTriggerExit(Collider c)
	{
		if (c.tag == Registry.Tag.Player || c.tag == Registry.Tag.Proxy)
		{
			targets.Remove(c.transform);
		}
	}
}