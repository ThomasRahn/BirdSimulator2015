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
		public Rigidbody TargetRigidBody;

		protected Camera cam;
		protected float velocity;

		protected virtual void Awake()
		{
	        cam = this.GetComponent<Camera>();
			// this will hopefully always work!
			TargetRigidBody = GameController.Player.rigidbody;
		}

		private void LateUpdate()
		{
			velocity = TargetRigidBody.velocity.magnitude;
			UpdatePosition();
            UpdateFieldOfView();
		}

        private void UpdateFieldOfView()
        {
            if (velocity > FOVThreshold)
            {
                float finalFOV = Mathf.Min(90, FOVCoefficient * velocity + 60);
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, finalFOV, Time.deltaTime * 10);
            }
            else
            {
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, 60, Time.deltaTime * 10);
            }
        }

		protected abstract void UpdatePosition();
	}
}
