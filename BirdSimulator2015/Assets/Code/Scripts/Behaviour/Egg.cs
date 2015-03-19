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
			Rigidbody egg = transform.parent.GetComponent<Rigidbody>();
			egg.useGravity = true;

			float distance = this.GetComponent<SphereCollider>().radius/1.5f;
			transform.parent.position = Vector3.MoveTowards(transform.parent.position, c.transform.position, distance);

			float eggHeight = transform.parent.GetComponent<MeshFilter>().mesh.bounds.extents.y;
			Vector3 initialPosition = transform.parent.position + transform.parent.up * eggHeight;

			links = ChainLinker.Link(initialPosition, c.transform.position, egg.GetComponent<Joint>(), c.attachedRigidbody);

			GetComponent<Collider>().enabled = false; // Prevent picking up the egg again
			player = c.GetComponent<PlayerState>();
			player.HeldEgg = this;
        }
    }

	public void Detach()
	{
		for(int i = 0; i < links.Count; i++)
		{
			Destroy(links[i]);
		}
		links.Clear();
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
