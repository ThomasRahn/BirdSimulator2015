using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour
{
    private Vector3 spawn;

    bool pickedup = false;

    void Awake()
    {
        spawn = this.transform.position;
    }

	void Start()
    {
	}
	
	void Update()
    {
        if (pickedup)
        {
            this.transform.parent.position = GameObject.FindGameObjectWithTag("Player").transform.position + Vector3.down * 3f;
        }

	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
            pickedup = true;
        }
        else if (c.name == "Lock")
        {
            pickedup = false;
            this.transform.parent.position = c.transform.position;
            this.transform.parent.rigidbody.isKinematic = true;
        }
    }
}
