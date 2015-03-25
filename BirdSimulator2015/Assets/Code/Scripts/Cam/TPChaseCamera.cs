using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class TPChaseCamera : TPCamera 
{
	public Vector3 Center {get; set;}
	public Vector3 Forward {get; set;}
	public float maxOffset;

	protected override void UpdatePosition()
	{
		Vector3 newPlanarPoint = target.transform.position - Radius * Forward;

		Vector3 planeToPoint = newPlanarPoint - Center;
		if(planeToPoint.magnitude > 0)
		{
			Vector3 toPoint = Vector3.Project(planeToPoint, Forward);
			float angle = Vector3.Angle(toPoint, Forward);
			float forwardDistance = angle > 90 ? -toPoint.magnitude : toPoint.magnitude;

			Center = Center + forwardDistance * Forward;

			toPoint = newPlanarPoint - Center;
			toPoint = toPoint.magnitude > maxOffset ? toPoint.normalized * maxOffset : toPoint;

			transform.position = Center + toPoint;
		}
		transform.forward = Forward;
	}
}
