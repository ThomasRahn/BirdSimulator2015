using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Egg : MonoBehaviour
{
	private static GameObject link;
    private Vector3 spawn;
	private List<GameObject> links;

    void Awake()
    {
		if(link == null)
		{
			link = Resources.Load("Misc/ChainLink") as GameObject;
		}
		links = new List<GameObject>();
        spawn = this.transform.position;
    }

	void Start()
    {
	}
	
	void Update()
    {
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
			Vector3 currentLinkPosition = this.transform.parent.renderer.bounds.max;
			Vector3 birdDirection = c.transform.position - currentLinkPosition;

			// Get any vector perpindicular to the direction towards the bird in order to get the Quaternion
			Vector3 forward = Vector3.RotateTowards(birdDirection, -birdDirection, Mathf.PI/2f, 0);
			Quaternion linkRotation = Quaternion.LookRotation(forward, birdDirection);
			
			GameObject currentLink = link;
			float linkHeight = link.renderer.bounds.max.y * 2;
			Rigidbody prevBody = transform.parent.rigidbody;
			prevBody.useGravity = true;
			
			while(Vector3.Distance(currentLinkPosition, c.transform.position) > linkHeight)
			{
				currentLink = GameObject.Instantiate(link, currentLinkPosition, linkRotation) as GameObject;
				currentLink.transform.parent = this.transform.parent;
				currentLinkPosition += currentLink.transform.up * linkHeight;
				links.Add(currentLink);
				
				ConnectJoint(prevBody.GetComponent<ConfigurableJoint>(), currentLink.rigidbody);
				prevBody = currentLink.rigidbody;
			}

			ConnectJoint(currentLink.GetComponent<ConfigurableJoint>(), c.rigidbody);
			currentLink.collider.enabled = false; // To prevent triggering fly backwards

			this.collider.enabled = false;
        }
    }

	private void ConnectJoint(ConfigurableJoint previous, Rigidbody next)
	{
		previous.connectedBody = next;
		previous.autoConfigureConnectedAnchor = true;

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
	}
}
