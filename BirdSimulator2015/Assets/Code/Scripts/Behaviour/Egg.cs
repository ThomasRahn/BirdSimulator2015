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
			Vector3 birdDirection = c.transform.position - this.transform.position;
			Vector3 forward = Vector3.RotateTowards(birdDirection, -birdDirection, Mathf.PI/2f, 0);
			Quaternion linkRotation = Quaternion.LookRotation(forward, birdDirection);
			
			Vector3 currentLinkPosition = this.transform.position;
			float linkHeight = link.renderer.bounds.max.y;
			
			GameObject currentLink = link;
			Rigidbody prevBody = transform.parent.rigidbody;
			prevBody.useGravity = true;
			
			while(Vector3.Distance(currentLinkPosition, c.transform.position) > linkHeight)
			{
				currentLink = GameObject.Instantiate(link, currentLinkPosition, linkRotation) as GameObject;
				currentLink.transform.parent = this.transform.parent;
				Debug.Log(currentLinkPosition);
				currentLinkPosition += currentLink.transform.up * linkHeight;
				
				currentLink.GetComponent<ConfigurableJoint>().connectedBody = prevBody;
				prevBody = currentLink.rigidbody;
			}
			
			ConfigurableJoint birdHook = currentLink.AddComponent<ConfigurableJoint>();
			birdHook.connectedBody = c.rigidbody;
			birdHook.xMotion = ConfigurableJointMotion.Locked;
			birdHook.yMotion = ConfigurableJointMotion.Locked;
			birdHook.zMotion = ConfigurableJointMotion.Locked;

			this.collider.enabled = false;
        }
    }
}
