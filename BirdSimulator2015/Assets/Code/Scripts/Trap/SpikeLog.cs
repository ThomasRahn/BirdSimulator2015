using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeLog : SwingOnceTrap 
{
    private const float TRIGGER_LOOKAHEAD = 100f;

	private void Start() 
	{
		Vector3 originalPosition = transform.position;

		Vector3 rotation = transform.parent.rotation.eulerAngles;
		rotation.x += 75;
		transform.parent.rotation = Quaternion.Euler(rotation);

		float halfLogWidth = GetComponent<CapsuleCollider>().bounds.size.x * 2;
		float hookHeight = GetComponent<CapsuleCollider>().bounds.size.y/2;

		Vector3 rightHook = transform.position + transform.right * halfLogWidth + transform.up * hookHeight;
		Vector3 leftHook = transform.position - transform.right * halfLogWidth + transform.up * hookHeight;

		Vector3 rightAnchor = transform.parent.position + transform.right * halfLogWidth;
		Vector3 leftAnchor = transform.parent.position - transform.right * halfLogWidth;

		Joint[] joints = GetComponents<Joint>();

		bodies.Add(GetComponent<Rigidbody>(), new TransformCopy(transform.position, transform.rotation));
		foreach(GameObject link in ChainLinker.Link(rightHook, rightAnchor, joints[0], null, false))
		{
			bodies.Add(link.GetComponent<Rigidbody>(), new TransformCopy(link.transform.position, link.transform.rotation));
		}

		foreach(GameObject link in ChainLinker.Link(leftHook, leftAnchor, joints[1], null, false))
		{
			bodies.Add(link.GetComponent<Rigidbody>(), new TransformCopy(link.transform.position, link.transform.rotation));
		}

        PlaceTrigger(originalPosition + this.transform.up * TRIGGER_LOOKAHEAD);
	}
}
