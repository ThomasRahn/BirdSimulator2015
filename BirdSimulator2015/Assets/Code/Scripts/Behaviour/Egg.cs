using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour
{
    private Vector3 spawn; 

    void Awake()
    {
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
            this.transform.parent.transform.parent = c.transform;
            this.transform.position = Vector3.zero;
            this.transform.localPosition = Vector3.zero;
            this.rigidbody.isKinematic = true;
            this.rigidbody.useGravity = false;
        }
    }
}
