using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SwingOnceTrap : MonoBehaviour 
{
    Vector3 OFFSET = new Vector3(-300f, -20f, 0f);

	protected struct TransformCopy
	{
		public Vector3 position;
		public Quaternion rotation;

		public TransformCopy(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}
	}

	protected Dictionary<Rigidbody, TransformCopy> bodies = new Dictionary<Rigidbody, TransformCopy>();

	protected void PlaceTrigger(Vector3 position)
	{
		GameObject trigger = ResourceFactory.GetInstance().GetPrefab(Registry.Prefab.TrapTrigger);

        trigger = GameObject.Instantiate(trigger, position + OFFSET, Quaternion.identity) as GameObject;
		trigger.transform.parent = this.transform.parent;
	}

	public void Swing()
	{
		foreach(Rigidbody rb in bodies.Keys)
		{
			rb.useGravity = true;
		}
	}

	public void ResetTrap()
	{
		Dictionary<Rigidbody, TransformCopy>.Enumerator enumerator = bodies.GetEnumerator();
		while(enumerator.MoveNext())
		{
			Rigidbody body = enumerator.Current.Key;
			TransformCopy original = enumerator.Current.Value;

			body.useGravity = false;
			body.angularVelocity = Vector3.zero;
			body.velocity = Vector3.zero;
			body.transform.position = original.position;
			body.transform.rotation = original.rotation;
		}
	}
}
