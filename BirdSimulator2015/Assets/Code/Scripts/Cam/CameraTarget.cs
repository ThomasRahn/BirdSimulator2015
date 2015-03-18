using UnityEngine;
using System.Collections;

namespace BirdSimulator2015.Code.Scripts.Cam
{
	/// <summary>
	/// Camera target that follows the bird position but rotates more slowly.
	/// </summary>
	public class CameraTarget : MonoBehaviour 
	{
		private GameObject bird;
		private bool followRotation;

		private void Awake()
		{
			followRotation = false;
		}

		private void Update() 
		{
			transform.position = bird.transform.position;

			if(followRotation)
			{
				Vector3 targetRotation = bird.transform.rotation.eulerAngles;
				if(targetRotation.z > 180)
				{
					targetRotation.z = Mathf.Clamp(targetRotation.z, 330, 360);
				}
				else
				{
					targetRotation.z = Mathf.Clamp(targetRotation.z, 0, 30);
				}

				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), 2*Time.deltaTime);
			}
		}

		public void ToggleRotation(bool enabled)
		{
			followRotation = enabled;
		}

		public void SetTarget(GameObject bird)
		{
			this.bird = bird;
			transform.position = bird.transform.position;
			transform.rotation = bird.transform.rotation;

			bird.GetComponent<PlayerInput>().Cameras = GetComponent<CameraContainer>();
		}
	}
}
