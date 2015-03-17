using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireballZone : MonoBehaviour
{
	public float Speed = 3f;
	public Transform Target;

	private List<Transform> targets = new List<Transform>();
	private Transform fireball;

	void Awake()
	{
		GameObject g = uLink.Network.Instantiate(uLink.Network.player, "Fireball", "Fireball", "Fireball", new Vector3(-1313f, -25f, -637f), Quaternion.identity, 0);
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

	void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player" || c.tag == "Proxy")
		{
			targets.Add(c.transform);
		}
	}
	
	void OnTriggerExit(Collider c)
	{
		if (c.tag == "Player" || c.tag == "Proxy")
		{
			targets.Remove(c.transform);
		}
	}
}