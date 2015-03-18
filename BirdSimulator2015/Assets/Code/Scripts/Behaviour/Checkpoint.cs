using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	public Transform Spawn;
	
	void Start()
	{
	}
	
	void Update()
	{
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.tag == Registry.Tag.Player)
		{
			Debug.Log("Set checkpoint as " + Spawn.position);
			GameController.SetLastCheckpoint(Spawn.position);
		}
	}
}
