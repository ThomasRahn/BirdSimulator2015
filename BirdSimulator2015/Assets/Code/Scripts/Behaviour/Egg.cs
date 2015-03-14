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
            //this.transform.parent.collider.enabled = false;
            this.GetComponent<Collider>().enabled = false;
            this.transform.parent.SetParent(c.transform, false);
            this.transform.parent.position = Vector3.zero;
            this.transform.parent.localPosition = c.transform.forward * 4f;
            this.transform.parent.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
