using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Tether : MonoBehaviour 
{
	public float tetherDistance;
	public float tetherForce;

	public GameObject attached1;
	public GameObject attached2;

	private LineRenderer line;

	private void Start()
	{
		line = GetComponent<LineRenderer>();
		attached1 = GameObject.FindGameObjectWithTag("Player");
		attached2 = GameObject.FindGameObjectWithTag("Proxy");
	}
	
	private void Update() 
	{
		transform.position = (attached1.transform.position + attached2.transform.position)/2;
		float distance = Vector3.Distance(attached1.transform.position, attached2.transform.position);
		Debug.Log(distance);
		if(distance > tetherDistance)
		{
			float scale = (distance - tetherDistance)/10;
			Vector3 towards1 = attached1.transform.position - attached2.transform.position;
			attached1.GetComponent<Rigidbody>().AddForce(-towards1 * tetherForce * scale);
		}

		line.SetPosition(0, attached1.transform.position);
		line.SetPosition(1, attached2.transform.position);
	}
}
