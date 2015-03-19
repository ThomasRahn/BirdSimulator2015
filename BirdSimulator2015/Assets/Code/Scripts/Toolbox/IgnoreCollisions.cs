using UnityEngine;
using System.Collections;

public class IgnoreCollisions : MonoBehaviour 
{
	private void OnCollisionEnter(Collision c)
	{
		Physics.IgnoreCollision(c.collider, this.GetComponent<Collider>());
	}
}
