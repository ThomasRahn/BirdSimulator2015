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
		
		private float shakeAmplitude = 0.01f;
		private GameObject target;

		protected virtual void Awake()
		{
            TargetRadius = Radius;
	        cam = this.GetComponent<Camera>();
			parent = transform.parent;
		}

		private void FixedUpdate()
		{
			velocity = target.GetComponent<Rigidbody>().velocity.magnitude;
			UpdatePosition();
            UpdateFieldOfView();
            UpdateRadius();
			collisionResolution();
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

		private void collisionResolution()
		{
			Vector3 towardsPlayer = target.transform.position - transform.position;
			RaycastHit hit;
			if(Physics.Raycast(target.transform.position, -towardsPlayer, out hit, towardsPlayer.magnitude))
			{
				reposition(hit, true);
			}
			else if(Physics.Raycast(transform.position, towardsPlayer, out hit, towardsPlayer.magnitude))
			{
				reposition(hit, false);
			}
		}

		private void reposition(RaycastHit hit, bool reverse)
		{
			float offset = (transform.position - target.transform.position).magnitude * Mathf.Tan(Mathf.PI/6);
			Vector3 rightOffset = transform.position + transform.right * offset;
			Vector3 leftOffset = transform.position - transform.right * offset;

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
				if(!Physics.Raycast(transform.position, rightDirection, rightDirection.magnitude) &&
				   !Physics.Raycast(transform.position, leftDirection, rightDirection.magnitude))
				{
					return;
				}
			}
			this.transform.position = hit.point + hit.normal * Camera.main.nearClipPlane * 2;
		}

		private void shake()
		{
			Vector3 position = transform.localPosition;
			position += Vector3.up * (Random.value - 0.5f) * shakeAmplitude;
			position += Vector3.right * (Random.value - 0.5f) * shakeAmplitude;
			transform.localPosition = position;
		}

		protected void positionBehind()
		{
			Vector3 localPosition = transform.localPosition;
			localPosition = -Vector3.forward * Radius + Vector3.up * UpOffset;
			transform.localPosition = localPosition;
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

		protected abstract void UpdatePosition();
	}
}
