using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeLog : SwingOnceTrap 
{
	private List<Rigidbody> links;

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

		links = new List<Rigidbody>();
		links.AddRange(
			ChainLinker.Link(rightHook, rightAnchor, joints[0], null, false).ConvertAll(l => {
				return l.GetComponent<Rigidbody>();
			})
		);
		links.AddRange(
			ChainLinker.Link(leftHook, leftAnchor, joints[1], null, false).ConvertAll(l => {
				return l.GetComponent<Rigidbody>();
			})
		);

		PlaceTrigger(originalPosition);

		Invoke("Swing", 3f);
	}
	
	public override void Swing()
	{
		GetComponent<Rigidbody>().useGravity = true;
		for(int i = 0; i < links.Count; i++)
		{
			links[i].useGravity = true;
        }
//		GetComponent<Rigidbody>().AddForce(transform.forward * 0.05f);
    }
}
