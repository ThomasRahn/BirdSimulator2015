using UnityEngine;
using System.Collections;

public class CameraPath : MonoBehaviour 
{
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}
