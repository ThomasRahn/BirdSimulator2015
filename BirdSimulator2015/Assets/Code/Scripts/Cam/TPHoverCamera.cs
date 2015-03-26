using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class TPHoverCamera : TPCamera
{
	private Vector3 targetPosition = Vector3.zero;
	private float radialUp;
	private float radialRadius;

	protected override void Awake ()
	{
		base.Awake();
		positionBehind();

		TPRadialCamera radial = GetComponent<TPRadialCamera>();
		radialUp = radial.UpOffset;
		radialRadius = radial.Radius;
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
			targetPosition = target.transform.position + Vector3.up * radialUp - target.transform.forward * radialRadius;
		}
		else // updown < -4
		{
			targetPosition = target.transform.position + Vector3.up * UpOffset - target.transform.forward * Radius;
		}

		if(Vector3.Distance(transform.position, targetPosition) > Registry.Constant.MIN_LERP_DISTANCE)
		{
			transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			transform.LookAt(target.transform.position);
		}
	}
}
