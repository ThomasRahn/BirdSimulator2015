using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour
{
    private Vector3 spawn;
	private FixedJoint birdHook;

    void Awake()
    {
        spawn = this.transform.position;
		GameObject lastLink = transform.parent.Find("ChainLink5").gameObject;
		birdHook = lastLink.GetComponent<FixedJoint>();
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
            //this.transform.parent.collider.enabled = false;
//            this.collider.enabled = false;
//            this.transform.parent.SetParent(c.transform, false);
//            this.transform.parent.position = Vector3.zero;
//            this.transform.parent.localPosition = c.transform.forward * 4f;
//            this.transform.parent.rigidbody.isKinematic = true;

			float height = birdHook.GetComponent<MeshFilter>().mesh.bounds.max.y;
			Vector3 position = c.transform.position;
			position.y -= height/2;

			birdHook.transform.position = position;
			birdHook.connectedBody = c.rigidbody;
        }
    }
}
