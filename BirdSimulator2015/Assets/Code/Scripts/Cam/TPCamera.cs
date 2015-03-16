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
        public float FOVThreshold = 25;
        public float UpOffset = 0.15f;
		public float FOVCoefficient = 0.75f;
		public float Radius = 1.0f;
		
		protected Camera cam;
		protected float velocity;
		protected Transform parent;
		
		private float shakeAmplitude = 0.01f;
		private GameObject target;
		private Renderer playerBody;

		protected virtual void Awake()
		{
	        cam = this.GetComponent<Camera>();
			parent = transform.parent;
		}

		private void LateUpdate()
		{
			velocity = target.GetComponent<Rigidbody>().velocity.magnitude;
			UpdatePosition();
            UpdateFieldOfView();
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

		private void collisionResolution()
		{
			Vector3 towardsPlayer = target.transform.position - transform.position;
			RaycastHit hitInfo;

			if(!playerBody.isVisible)
			{
				if(Physics.Raycast(transform.position, towardsPlayer, out hitInfo, towardsPlayer.magnitude) && hitInfo.collider.gameObject != target)
				{
					Vector3 toObstruction = Vector3.Project(hitInfo.point - target.transform.position, hitInfo.normal) * 0.75f;
					Vector3 parallelObstruction = (hitInfo.point - (target.transform.position + toObstruction)).normalized;

					// Make a triangle to calculate where the new camera position should be to keep the same radius
					float distanceAlongObstruction = Mathf.Sqrt(Mathf.Pow(Radius, 2) - Mathf.Pow(toObstruction.magnitude, 2));

					Vector3 finalPosition = target.transform.position + toObstruction + distanceAlongObstruction * parallelObstruction;
					finalPosition -= Vector3.up * UpOffset;

					parent.transform.rotation = Quaternion.LookRotation(target.transform.position - finalPosition);
				}
			}
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

			playerBody = root.Find("Raven/Raven").GetComponent<Renderer>();

			Rigidbody targetRigidbody = root.GetComponentInChildren<Rigidbody>();
			this.target = targetRigidbody.gameObject;
		}

		protected abstract void UpdatePosition();
	}
}
