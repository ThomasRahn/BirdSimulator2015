using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour
{
    private Vector3 spawn;
	private static GameObject link;

    void Awake()
    {
		if(link == null)
		{
			link = Resources.Load("Misc/ChainLink") as GameObject;
		}
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
			Vector3 currentLinkPosition = this.transform.parent.position 
										+ this.transform.parent.up * this.transform.parent.renderer.bounds.max.y;
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
				Debug.Log(currentLinkPosition);
				currentLinkPosition += currentLink.transform.up * linkHeight;
				
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
}
