using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class TPHoverCamera : TPCamera
{
	private Vector3 targetPosition = Vector3.zero;

	protected override void Awake ()
	{
		base.Awake();
		positionBehind();
	}

	protected override void UpdatePosition()
	{
		float updown = target.GetComponent<Rigidbody>().velocity.y;
		if(updown > 4)
		{
			targetPosition = target.transform.position - Vector3.up * UpOffset - target.transform.forward * Radius;
		}
		else if (updown > -4)
		{
			targetPosition = target.transform.position - target.transform.forward * Mathf.Sqrt(UpOffset*UpOffset + Radius*Radius);
		}
		else // updown < -4
		{
			targetPosition = target.transform.position + Vector3.up * UpOffset - target.transform.forward * Radius;
		}

		if(Vector3.Distance(transform.position, targetPosition) > 0.2f)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			transform.LookAt(target.transform.position);
		}
	}
}
