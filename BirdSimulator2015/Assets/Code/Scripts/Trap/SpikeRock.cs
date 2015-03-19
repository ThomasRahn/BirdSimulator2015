using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeRock : SwingOnceTrap 
{
	private List<Rigidbody> links;
	
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
		
		links = new List<Rigidbody>();

		links.AddRange(
			ChainLinker.Link(hook, anchor, joint, null, false).ConvertAll(l => {
			return l.GetComponent<Rigidbody>();
			})
		);
		
		PlaceTrigger(originalPosition);
	}
	
	public override void Swing()
	{
		GetComponent<Rigidbody>().useGravity = true;
		for(int i = 0; i < links.Count; i++)
		{
			links[i].useGravity = true;
		}
	}
}

