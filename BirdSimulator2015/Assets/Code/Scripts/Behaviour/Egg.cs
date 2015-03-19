using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Egg : MonoBehaviour
{
	private List<GameObject> links;
	private Vector3 spawn;
	private PlayerState player;

    void Awake()
    {
		links = new List<GameObject>();
    }

	private void Start()
    {
		spawn = transform.parent.position;
	}
	
    void OnTriggerEnter(Collider c)
    {
        if (c.tag == Registry.Tag.Player)
        {
			// Set up the egg for carrying and link for copying
			GameObject link = ResourceFactory.GetInstance().GetChainLink();
			float linkHeight = link.GetComponent<Renderer>().bounds.max.y * 2;
			Rigidbody prevBody = transform.parent.GetComponent<Rigidbody>();
			prevBody.useGravity = true;

			float towardsPlayerDistance = this.GetComponent<SphereCollider>().radius/1.5f;
			transform.parent.position = Vector3.MoveTowards(transform.parent.position, c.transform.position, towardsPlayerDistance);

			float eggHeight = transform.parent.GetComponent<MeshFilter>().mesh.bounds.extents.y;
			Vector3 currentLinkPosition = transform.parent.position + (transform.parent.up * (eggHeight + linkHeight/2));
			Vector3 birdDirection = c.transform.position - currentLinkPosition;

			// Get any vector perpindicular to the direction towards the bird in order to get the Quaternion
			Vector3 forward = Vector3.RotateTowards(birdDirection, -birdDirection, Mathf.PI/2f, 0);
			Quaternion linkRotation = Quaternion.LookRotation(forward, birdDirection);

			GameObject currentLink = null;
			while(Vector3.Distance(currentLinkPosition, c.transform.position) > linkHeight)
			{
				currentLink = GameObject.Instantiate(link, currentLinkPosition, linkRotation) as GameObject;
				currentLink.transform.parent = this.transform.parent;
				currentLinkPosition += currentLink.transform.up * linkHeight;
				links.Add(currentLink);
				
				ConnectJoint(prevBody.GetComponent<ConfigurableJoint>(), currentLink.GetComponent<Rigidbody>());
				prevBody = currentLink.GetComponent<Rigidbody>();
			}

			ConnectJoint(currentLink.GetComponent<ConfigurableJoint>(), c.GetComponent<Rigidbody>());
			currentLink.GetComponent<ConfigurableJoint>().autoConfigureConnectedAnchor = true; // Attach last link to bird using autoconfigure
			currentLink.GetComponent<Collider>().enabled = false; // To prevent triggering fly backwards

			GetComponent<Collider>().enabled = false; // Prevent picking up the egg again
			player = c.GetComponent<PlayerState>();
			player.HeldEgg = this;
        }
    }

	private void ConnectJoint(ConfigurableJoint previous, Rigidbody next)
	{
		previous.connectedBody = next;

		previous.xMotion = ConfigurableJointMotion.Locked;
		previous.yMotion = ConfigurableJointMotion.Locked;
		previous.zMotion = ConfigurableJointMotion.Locked;
	}

	public void Detach()
	{
		for(int i = 0; i < links.Count; i++)
		{
			Destroy(links[i]);
		}
		GetComponentInParent<Rigidbody>().useGravity = false;
		player.HeldEgg = null;
	}

	public void Reset()
	{
		Detach();
		GetComponent<Collider>().enabled = true;
		transform.parent.position = spawn;
	}
}
