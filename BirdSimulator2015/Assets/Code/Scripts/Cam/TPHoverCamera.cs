using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class TPHoverCamera : TPCamera
{
	private Vector3 targetPosition = Vector3.zero;
	private float radialUp;
	private float radialRadius;

    private const float UPDOWN = 2f;

	protected override void Awake()
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
        if (updown > UPDOWN)
		{
			targetPosition = target.transform.position - Vector3.up * (UpOffset / 2) - target.transform.forward * Radius;
		}
        else if (updown > -UPDOWN)
		{
			targetPosition = target.transform.position + Vector3.up * radialUp - target.transform.forward * radialRadius;
		}
        else // updown < -UPDOWN
		{
            targetPosition = target.transform.position + Vector3.up * (UpOffset / 2) - target.transform.forward * Radius;
		}

		if (Vector3.Distance(transform.position, targetPosition) > Registry.Constant.MIN_LERP_DISTANCE)
		{
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
			transform.LookAt(target.transform.position);
		}
	}
}
