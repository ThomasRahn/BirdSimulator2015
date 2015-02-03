using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	void Start()
	{
	}

	void Update()
	{
		Vector3 pos = this.transform.position;
		pos.x += Input.GetAxis("Horizontal");
		pos.z += Input.GetAxis("Vertical");

		this.transform.position = pos;
	}
}
