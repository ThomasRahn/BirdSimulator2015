using UnityEngine;
using System.Collections;

public class IgnoreCollisions : MonoBehaviour 
{
	private Collider col;

	private void Awake()
	{
		this.col = this.GetComponent<Collider>();
	}

	private void OnCollisionEnter(Collision c)
	{
		Physics.IgnoreCollision(c.collider, this.col);
	}

	private void OnCollisionStay(Collision c)
	{
		Physics.IgnoreCollision(c.collider, this.col);
	}
}
