using UnityEngine;
using System.Collections;

namespace BirdSimulator2015.Code.Scripts.Cam
{
	/// <summary>
	/// Third person camera.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public abstract class TPCamera : MonoBehaviour 
	{
        public float FOVThreshold;
        public float UpOffset;
		public float FOVCoefficient;
        public float Radius;
		[HideInInspector] public float TargetRadius;

		protected Camera cam;
		protected float velocity;
		protected Transform parent;
		protected GameObject target;
		protected float moveSpeed = 3f;
		
		private float shakeAmplitude = 0.01f;

		protected virtual void Awake()
		{
            TargetRadius = Radius;
	        cam = this.GetComponent<Camera>();
			parent = transform.parent;
		}

		private void FixedUpdate()
		{
			velocity = target.GetComponent<Rigidbody>().velocity.magnitude;
			Vector3 finalPosition = UpdatePosition();
            UpdateFieldOfView();
            UpdateRadius();
			collisionResolution(finalPosition);
		}

        private void UpdateFieldOfView()
        {
            if (velocity > FOVThreshold)
            {
				if (velocity > FOVThreshold * 1.1f)
				{
					shake();
				}
                float finalFOV = Mathf.Min(90, FOVCoefficient * velocity + 60);
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, finalFOV, Time.deltaTime * 10);
            }
            else
            {
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, 60, Time.deltaTime * 10);
            }
        }

        private void UpdateRadius()
        {
            if (Radius != TargetRadius)
            {
                Radius = Mathf.Lerp(Radius, TargetRadius, Time.deltaTime);
            }
        }

		private void collisionResolution(Vector3 finalPosition)
		{
			Vector3 towardsPlayer = target.transform.position - finalPosition;
			RaycastHit hit;
			if(Physics.Raycast(target.transform.position, -towardsPlayer, out hit, towardsPlayer.magnitude))
			{
				reposition(finalPosition, hit, true);
			}
			else if(Physics.Raycast(finalPosition, towardsPlayer, out hit, towardsPlayer.magnitude))
			{
				reposition(finalPosition, hit, false);
			}
		}

		private void reposition(Vector3 finalPosition, RaycastHit hit, bool reverse)
		{
			float offset = (finalPosition - target.transform.position).magnitude * Mathf.Tan(Mathf.PI/6);
			Vector3 rightOffset = finalPosition + transform.right * offset;
			Vector3 leftOffset = finalPosition - transform.right * offset;

			Vector3 rightDirection = target.transform.position - rightOffset;
			Vector3 leftDirection = target.transform.position - leftOffset;
			if(reverse)
			{
				if(!Physics.Raycast(target.transform.position, -rightDirection, rightDirection.magnitude) &&
				   !Physics.Raycast(target.transform.position, -leftDirection, rightDirection.magnitude))
				{
					return;
				}
			}
			else
			{
				if(!Physics.Raycast(finalPosition, rightDirection, rightDirection.magnitude) &&
				   !Physics.Raycast(finalPosition, leftDirection, rightDirection.magnitude))
				{
					return;
				}
			}

			Vector3 targetPosition = hit.point + hit.normal * Camera.main.nearClipPlane * 2;
			if(Vector3.Distance(transform.position, targetPosition) > Registry.Constant.MIN_LERP_DISTANCE)
			{
				transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			}
		}

		private void shake()
		{
			Vector3 position = transform.localPosition;
			position += Vector3.up * (Random.value - 0.5f) * shakeAmplitude;
			position += Vector3.right * (Random.value - 0.5f) * shakeAmplitude;
			transform.localPosition = position;
		}

		protected Vector3 positionBehind()
		{
			Vector3 targetLocalPosition = transform.localPosition;
			targetLocalPosition = -Vector3.forward * Radius + Vector3.up * UpOffset;
			if(Vector3.Distance(transform.localPosition, targetLocalPosition) > Registry.Constant.MIN_LERP_DISTANCE)
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
			}
			if(Quaternion.Angle(transform.localRotation, Quaternion.identity) > Registry.Constant.MIN_LERP_DISTANCE)
			{
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
			}

			return transform.parent.position + transform.rotation * targetLocalPosition;
		}

		public void SetTarget(GameObject target)
		{
			Transform root = target.transform;
			while(root.parent != null)
			{
				root = root.parent;
			}

			Rigidbody targetRigidbody = root.GetComponentInChildren<Rigidbody>();
			this.target = targetRigidbody.gameObject;
		}

		protected abstract Vector3 UpdatePosition();
	}
}
