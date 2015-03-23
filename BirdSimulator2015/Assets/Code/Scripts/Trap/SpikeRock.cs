using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeRock : SwingOnceTrap 
{
	private void Start() 
	{
		Vector3 originalPosition = transform.position;
		
		Vector3 rotation = transform.parent.rotation.eulerAngles;
		rotation.x += 75;
		transform.parent.rotation = Quaternion.Euler(rotation);
		
		float hookHeight = GetComponent<SphereCollider>().bounds.extents.y/2;
		
		Vector3 hook = transform.position + transform.up * hookHeight;
		Vector3 anchor = transform.parent.position;
		
		Joint joint = GetComponent<Joint>();

		bodies.Add(GetComponent<Rigidbody>(), new TransformCopy(transform.position, transform.rotation));
		foreach(GameObject link in ChainLinker.Link(hook, anchor, joint, null, false))
		{
			bodies.Add(link.GetComponent<Rigidbody>(), new TransformCopy(link.transform.position, link.transform.rotation));
		}
		
		PlaceTrigger(originalPosition);
	}
}

