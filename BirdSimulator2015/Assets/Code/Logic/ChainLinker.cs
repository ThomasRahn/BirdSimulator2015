using UnityEngine;
using System.Collections.Generic;

public class ChainLinker
{
	public static List<GameObject> Link(Vector3 currentPosition, Vector3 finalPosition, Joint hook, Rigidbody anchor, bool gravity)
	{
		List<GameObject> links = new List<GameObject>();

		GameObject link = ResourceFactory.GetInstance().GetChainLink();
		float linkHeight = link.GetComponent<Renderer>().bounds.max.y * 2;

		// Get any vector perpindicular to the direction towards the bird in order to get the Quaternion
		Vector3 direction = finalPosition - currentPosition;
		Vector3 forward = Vector3.RotateTowards(direction, -direction, Mathf.PI/2f, 0);
		Quaternion linkRotation = Quaternion.LookRotation(forward, direction);
        
        GameObject currentLink = null;
		ConfigurableJoint prevBody = null;
		while(Vector3.Distance(currentPosition, finalPosition) > linkHeight)
		{
			currentLink = GameObject.Instantiate(link, currentPosition, linkRotation) as GameObject;
			currentLink.transform.parent = hook.transform;

			currentPosition += currentLink.transform.up * linkHeight;

			links.Add(currentLink);

			Rigidbody currentBody = currentLink.GetComponent<Rigidbody>();
			currentBody.useGravity = gravity;
			if(hook.connectedBody == null)
			{
				hook.connectedBody = currentBody;
			}
			else
			{
				ConnectJoint(prevBody, currentBody);
			}
			prevBody = currentLink.GetComponent<ConfigurableJoint>();
		}

		if(anchor != null)
		{
			ConnectJoint(currentLink.GetComponent<ConfigurableJoint>(), anchor);
		}
		else
		{
			currentLink.GetComponent<ConfigurableJoint>().connectedAnchor = finalPosition;
		}
		currentLink.GetComponent<ConfigurableJoint>().autoConfigureConnectedAnchor = true;

		return links;
	}

	private static void ConnectJoint(ConfigurableJoint previous, Rigidbody next)
	{
		previous.connectedBody = next;

		previous.xMotion = ConfigurableJointMotion.Locked;
		previous.yMotion = ConfigurableJointMotion.Locked;
		previous.zMotion = ConfigurableJointMotion.Locked;
    }
}
