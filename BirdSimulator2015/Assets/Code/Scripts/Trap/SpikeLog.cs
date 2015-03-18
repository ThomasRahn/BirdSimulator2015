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

		Vector3 rightHook = transform.position + transform.right * 10f + transform.up * 2f;
		Vector3 leftHook = transform.position - transform.right * 10f + transform.up * 2f;
		Joint[] joints = GetComponents<Joint>();

		links = new List<Rigidbody>();

		CreateChain(rightHook, transform.parent.position + transform.right * 10f, joints[0]);
		CreateChain(leftHook, transform.parent.position - transform.right * 10f, joints[1]);

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
    
	private void CreateChain(Vector3 currentPosition, Vector3 finalPosition, Joint attachPoint)
	{
		GameObject link = ResourceFactory.GetInstance().GetChainLink();
		float linkHeight = link.GetComponent<Renderer>().bounds.max.y * 10;

		GameObject currentLink = null;
		ConfigurableJoint prevLink = null;
		bool rotate = false;
		while(Vector3.Distance(currentPosition, finalPosition) > linkHeight)
		{
			currentLink = GameObject.Instantiate(link, currentPosition, transform.rotation) as GameObject;
			currentLink.transform.parent = this.transform.parent;

			currentLink.transform.localScale = currentLink.transform.localScale * 5;

			currentPosition += currentLink.transform.up * linkHeight; // Move towards final point
			currentLink.transform.RotateAround(currentLink.transform.position, currentLink.transform.up, rotate ? 90 : 0);
			rotate = !rotate;
			
			Rigidbody currentRigidbody = currentLink.GetComponent<Rigidbody>();
			links.Add(currentRigidbody);
			if(attachPoint.connectedBody == null)
			{
				attachPoint.connectedBody = currentRigidbody;
				attachPoint.autoConfigureConnectedAnchor = true;
			}
			else
			{
				prevLink.connectedBody = currentLink.GetComponent<Rigidbody>();
			}
			prevLink = currentLink.GetComponent<ConfigurableJoint>();
			prevLink.autoConfigureConnectedAnchor = true;
			prevLink.GetComponent<Rigidbody>().useGravity = false;
		}

		prevLink.connectedAnchor = finalPosition;
	}
}
