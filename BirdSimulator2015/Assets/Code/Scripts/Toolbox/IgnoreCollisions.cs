using UnityEngine;
using System.Collections;

public class IgnoreCollisions : MonoBehaviour 
{
	private Collider collider;

	private void Awake()
	{
		this.collider = this.GetComponent<Collider>();
	}

	private void OnCollisionEnter(Collision c)
	{
		Physics.IgnoreCollision(c.collider, this.collider);
	}

	private void OnCollisionStay(Collision c)
	{
		Physics.IgnoreCollision(c.collider, this.collider);
	}
}
