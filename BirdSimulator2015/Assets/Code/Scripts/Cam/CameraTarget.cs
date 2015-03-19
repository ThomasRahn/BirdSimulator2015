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
		private float turn_speed = 5.0f;

		private void Awake()
		{
			followRotation = false;
		}

		private void FixedUpdate() 
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

				//Want to turn faster then you ascend/descend, but still slow down after turning.
				Vector3 turn_vector = new Vector3(transform.eulerAngles.x, targetRotation.y, transform.eulerAngles.z);

				if((turn_vector.y - transform.eulerAngles.y) < 20)
				{
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(turn_vector), 2.0f * Time.deltaTime);
				}
				else
				{
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(turn_vector), turn_speed * Time.deltaTime);
				}

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
